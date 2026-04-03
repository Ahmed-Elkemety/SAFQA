using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Dtos.AccountDto.User;
namespace SAFQA.BLL.Managers.AccountManager.Auth
{
    public interface IAuthUser
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto, string deciceId);
        Task<AuthResult> LoginAsync(LoginDto dto , string deciceId ,string role);
        Task<AuthResult> RefreshTokenAsync(string refreshToken , string deciceId);
        Task<AuthResult> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<AuthResult> ResendRegistrationOtpAsync(string email);

        Task<AuthResult> RequestPasswordResetAsync(string email);
        Task<AuthResult> VerifyOtpAsync(VerifyOtpDto dto);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResult> ResendOtpAsync(string email);
        Task<AuthResult> SignOutAllDevicesAsync(string userId);


    }
}
