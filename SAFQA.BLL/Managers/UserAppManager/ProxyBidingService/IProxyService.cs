using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.UserAppDto.ProxyBidding;
using SAFQA.BLL.Managers.AccountManager.Auth;

namespace SAFQA.BLL.Managers.UserAppManager.ProxyBidingService
{
    public interface IProxyService
    {
        Task<AuthResult> ActivateAsync(int auctionId, string userId);
        Task<AuthResult> DeactivateAsync(int auctionId, string userId);
        Task<AuthResult> CreateAsync(CreateProxyDto dto, string userId);
        Task<AuthResult> UpdateAsync(UpdateProxyDto dto, string userId);
    }
}
