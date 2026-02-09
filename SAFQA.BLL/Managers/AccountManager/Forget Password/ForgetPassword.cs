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
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            // حذف OTPs القديمة
            await CleanupOtpsAsync(user.Id);

            var record = await _context.PasswordResetOtps
                .Where(x =>
                    x.UserId == user.Id &&
                    !x.IsUsed &&
                    x.Expiration > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (record == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid or expired OTP"
                };

            // تحقق من عدد المحاولات
            var maxAttempts = int.Parse(_configuration["Security:MaxOtpAttempts"]);

            if (record.Attempts >= maxAttempts)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Too many attempts"
                };

            var hashedInput = Helper.HashOtp(dto.Code,
                _configuration["Security:OtpSecret"]);

            if (record.CodeHash != hashedInput)
            {
                record.Attempts++;
                await _context.SaveChangesAsync();

                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid OTP"
                };
            }

            record.IsUsed = true;

            var sessionToken = Helper.GenerateSessionToken();

            await _context.SaveChangesAsync();

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

        public async Task<AuthResult> ResendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            var lastOtp = await _context.PasswordResetOtps
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            // cooldown
            var cooldown = int.Parse(_configuration["Security:OtpCooldownSeconds"]);

            if (lastOtp != null &&
                lastOtp.CreatedAt > DateTime.UtcNow.AddSeconds(-cooldown))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Please wait before requesting another OTP"
                };
            }

            // resend limit per hour
            var resendLimit = int.Parse(_configuration["Security:MaxResendPerHour"]);

            var resendCount = await _context.PasswordResetOtps
                .CountAsync(x =>
                    x.UserId == user.Id &&
                    x.CreatedAt > DateTime.UtcNow.AddHours(-1));

            if (resendCount >= resendLimit)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Too many requests, try again later"
                };
            }

            // delete old
            var oldOtps = _context.PasswordResetOtps
                .Where(x => x.UserId == user.Id);

            _context.PasswordResetOtps.RemoveRange(oldOtps);
            await _context.SaveChangesAsync();

            var code = Helper.GenerateOtp();
            var hash = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            var otp = new PasswordResetOtp
            {
                UserId = user.Id,
                CodeHash = hash,
                Expiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Security:OtpExpiryMinutes"]))
            };

            _context.PasswordResetOtps.Add(otp);
            await _context.SaveChangesAsync();

            await _emailSender.SendEmailAsync(
                user.Email,
                "Your OTP Code",
                $"<h2>Your OTP is: {code}</h2>");

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP resent successfully"
            };
        }
    }
}
