using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.AccountDto;
namespace SAFQA.BLL.Managers.AccountManager
{
    public interface IAuthUser
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto);
        Task<AuthResult> LoginAsync(LoginDto dto);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
    }
}
