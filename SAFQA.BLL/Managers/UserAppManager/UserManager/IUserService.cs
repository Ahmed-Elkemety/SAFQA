using SAFQA.BLL.Dtos.UserAppDto.AccountDto;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using SAFQA.BLL.Dtos.UserAppDto.ProfileDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;
namespace SAFQA.BLL.Managers.UserAppManager
{
    public interface IUserService
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<int> GetBlockedUsersCountAsync();
        bool ChangeStatus(string userId);

        Task<(AuthResult, UserProfileDto?)> GetProfile(string userId);
        Task<(AuthResult, UserAccountDto?)> GetAccount(string userId);
        Task<AuthResult> EditAccount(string userId, EditAccountDto dto);
    }
}
