using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Managers.AccountManager.Auth;

namespace SAFQA.BLL.Managers.UserAppManager.InteractionService
{
    public interface IUserInteractionService
    {
        Task<AuthResult> AddFavoriteAsync(int auctionId, string userId);
        Task<AuthResult> AddViewAsync(int auctionId, string userId, string deviceType);
    }
}
