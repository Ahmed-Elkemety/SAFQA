using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.AccountDto.User;
namespace SAFQA.BLL.Managers.AccountManager
{
    public interface IAuthUser
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto, string deciceId);
        Task<AuthResult> LoginAsync(LoginDto dto , string deciceId);
        Task<AuthResult> GoogleLoginAsync(string idToken, string deviceId);
        Task<AuthResult> FacebookLoginAsync(string accessToken, string deviceId);
        Task<AuthResult> RefreshTokenAsync(string refreshToken , string deciceId);
    }
}
