using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Dtos.AccountDto.User;
namespace SAFQA.BLL.Managers.AccountManager.Auth
{
    public interface IAuthUser
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto, string deciceId);
        Task<AuthResult> LoginAsync(LoginDto dto , string deciceId);
        Task<AuthResult> RefreshTokenAsync(string refreshToken , string deciceId);
        Task<AuthResult> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<AuthResult> ResendRegistrationOtpAsync(string email);


    }
}
