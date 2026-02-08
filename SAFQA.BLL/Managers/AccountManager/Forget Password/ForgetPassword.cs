using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Help;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.AccountManager.Forget_Password
{
    public class ForgetPassword: IForgetPassword
    {
        #region Dependency Injection , UserManagement & SignInManager in Identity , IConfiguration To Access To App Settings 
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly SAFQA_Context _context;
        private readonly SAFQA.BLL.Managers.AccountManager.Email_Sender.IEmailSender _emailSender;
        public ForgetPassword(UserManager<User> userManager
            , SignInManager<User> signInManager
            , IConfiguration configuration
            , SAFQA_Context context,
            SAFQA.BLL.Managers.AccountManager.Email_Sender.IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }
        #endregion

        public async Task<AuthResult> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            // Cleanup أي OTPs قديمة
            await CleanupOtpsAsync(user.Id);

            // Generate OTP + hash
            var code = Helper.GenerateOtp();
            var hashed = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            // Save new OTP
            _context.PasswordResetOtps.Add(new PasswordResetOtp
            {
                UserId = user.Id,
                CodeHash = hashed,
                Expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Security:OtpExpiryMinutes"])),
                IsUsed = false
            });
            await _context.SaveChangesAsync();

            // Send OTP Email
            var body = $"<h2>Your OTP code is: {code}</h2><p>It expires in 10 minutes.</p>";
            await _emailSender.SendEmailAsync(user.Email, "Password Reset Code", body);

            return new AuthResult { IsSuccess = true, Message = "OTP sent to your email" };
        }

        public async Task<AuthResult> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            // Cleanup أي OTPs منتهية
            await CleanupOtpsAsync(user.Id);

            var hashedInput = Helper.HashOtp(dto.Code, _configuration["Security:OtpSecret"]);

            var record = await _context.PasswordResetOtps
                .Where(x => x.UserId == user.Id && !x.IsUsed && x.Expiration > DateTime.UtcNow)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.CodeHash == hashedInput);

            if (record == null)
                return new AuthResult { IsSuccess = false, Message = "Invalid or expired OTP" };

            record.IsUsed = true;
            await _context.SaveChangesAsync();

            var sessionToken = Helper.GenerateSessionToken();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP verified",
                Token = sessionToken
            };
        }

        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            // TODO: تحقق من sessionToken لو عايب أمان أكتر

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Reset failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

            await _emailSender.SendEmailAsync(user.Email, "Password Reset Successful", "<p>Your password has been changed successfully.</p>");

            return new AuthResult { IsSuccess = true, Message = "Password reset successfully" };
        }

        private async Task CleanupOtpsAsync(string userId)
        {
            var expiredOtps = _context.PasswordResetOtps
                .Where(x => x.UserId == userId && (x.IsUsed || x.Expiration <= DateTime.UtcNow));

            _context.PasswordResetOtps.RemoveRange(expiredOtps);
            await _context.SaveChangesAsync();
        }
    }
}
