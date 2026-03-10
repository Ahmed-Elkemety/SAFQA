using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Managers.AccountManager.Auth;

namespace SAFQA.BLL.Managers.AccountManager.Forget_Password
{
    public interface IForgetPassword
    {
        Task<AuthResult> RequestPasswordResetAsync(string email);
        Task<AuthResult> VerifyOtpAsync(VerifyOtpDto dto);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResult> ResendOtpAsync(string email);
        Task<AuthResult> SignOutAllDevicesAsync(string userId);

    }
}
