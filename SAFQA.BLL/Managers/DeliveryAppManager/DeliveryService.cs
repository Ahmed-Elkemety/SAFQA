using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Dtos.DeliveryDto;
using SAFQA.BLL.Help;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.AccountManager.Email_Sender;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Delivery;

namespace SAFQA.BLL.Managers.DeliveryAppManager
{
    public class DeliveryService: IDeliveryService
    {
        private readonly IDeliveryRepo _delivery;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public DeliveryService(IDeliveryRepo delivery , UserManager<User> userManager , IConfiguration configuration , IEmailSender emailSender)
        {
            _delivery = delivery;
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<(AuthResult, List<DeliveryDto>)> GetSellerDeliveries(string sellerId)
        {
            var deliveries = await _delivery.GetDeliveries(sellerId);

            if (deliveries == null || !deliveries.Any())
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "No deliveries found"
                }, new List<DeliveryDto>());
            }

            var result = deliveries.Select(d => new DeliveryDto
            {
                Id = d.Id,
                Status = d.Status,

                UserNumber = d.User?.PhoneNumber ?? "",
                UserEmail = d.User?.Email ?? "",

                AuctionTitle = d.Auction?.Title ?? "",
                FinalPrice = d.Auction?.FinalPrice ?? 0
            }).ToList();

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Success"
            }, result);
        }

        public async Task<AuthResult> RequestLoginOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            await _delivery.CleanupAsync(user.Id);

            var code = Helper.GenerateOtp();
            var hashed = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            await _delivery.AddAsync(new LoginOtp
            {
                UserId = user.Id,
                CodeHash = hashed,
                Expiration = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                Attempts = 0
            });

            await _delivery.SaveChangesAsync();

            await _emailSender.SendEmailAsync(user.Email, "Login OTP", $"Code: {code}");

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP sent"
            };
        }

        private async Task<string> GenerateTokensAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("SecurityStamp", user.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );
            return (new JwtSecurityTokenHandler().WriteToken(token));
        }
        public async Task<AuthResult> VerifyLoginOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            var record = await _delivery.GetLatestValidOtpAsync(user.Id);

            if (record == null)
                return new AuthResult { IsSuccess = false, Message = "Invalid or expired OTP" };

            if (record.Attempts >= 3)
                return new AuthResult { IsSuccess = false, Message = "Too many attempts" };

            var hashedInput = Helper.HashOtp(dto.Code, _configuration["Security:OtpSecret"]);

            if (record.CodeHash != hashedInput)
            {
                record.Attempts++;
                await _delivery.SaveChangesAsync();

                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid OTP"
                };
            }

            record.IsUsed = true;
            await _delivery.SaveChangesAsync();

            var token = GenerateTokensAsync(user);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Login successful",
                Token = await token
            };
        }

        // 🔥 STEP 2
        public async Task<AuthResult> Step2Async(int auctionId)
        {
            var delivery = await _delivery.GetByAuctionIdAsync(auctionId);

            if (delivery == null)
                return Fail("Delivery not found");

            delivery.Status = DeliveryStatus.Inprogress;

            await _delivery.AddTrackingAsync(new OrderTracking
            {
                AuctionId = auctionId,
                Step = TrackingStep.InProgress,
                Date = DateTime.UtcNow,
                IsCompleted = true
            });

            await _delivery.SaveChangesAsync();

            return Success("Step 2 completed");
        }

        // 📞 STEP 3
        public async Task<AuthResult> Step3Async(int auctionId, string contact)
        {
            var delivery = await _delivery.GetByAuctionIdAsync(auctionId);

            if (delivery == null)
                return Fail("Delivery not found");

            delivery.Status = DeliveryStatus.shipping;
            delivery.ContactNumber = contact;

            await _delivery.AddTrackingAsync(new OrderTracking
            {
                AuctionId = auctionId,
                Step = TrackingStep.Shipping,
                Date = DateTime.UtcNow,
                IsCompleted = true
            });

            await _delivery.SaveChangesAsync();

            return Success("Step 3 completed");
        }

        // 🖼️ STEP 4
        public async Task<AuthResult> Step4Async(int auctionId, IFormFile image)
        {
            var delivery = await _delivery.GetByAuctionIdAsync(auctionId);
            if (image == null)
                throw new Exception("Image is empty");

            byte[] imageBytes;

            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }
            if (delivery == null)
                return Fail("Delivery not found");

            delivery.Status = DeliveryStatus.Deliverd;
            delivery.ProfImage = imageBytes;

            await _delivery.AddTrackingAsync(new OrderTracking
            {
                AuctionId = auctionId,
                Step = TrackingStep.Delivery,
                Date = DateTime.UtcNow,
                IsCompleted = true
            });

            await _delivery.SaveChangesAsync();

            return Success("Step 4 completed");
        }

        // ✅ STEP 5
        public async Task<AuthResult> Step5Async(int auctionId)
        {
            var delivery = await _delivery.GetByAuctionIdAsync(auctionId);

            if (delivery == null)
                return Fail("Delivery not found");

            delivery.Status = DeliveryStatus.Failed;

            await _delivery.SaveChangesAsync();

            return Success("Delivery not completed");
        }

        // 🔧 helpers
        private AuthResult Success(string msg) =>
            new() { IsSuccess = true, Message = msg };

        private AuthResult Fail(string msg) =>
            new() { IsSuccess = false, Message = msg };
    }
}
