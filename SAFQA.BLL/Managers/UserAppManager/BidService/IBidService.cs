using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.UserAppDto.BidDto;
using SAFQA.BLL.Managers.AccountManager.Auth;

namespace SAFQA.BLL.Managers.UserAppManager.BidService
{
    public interface IBidService
    {
        Task PlaceManualBid(string userId, int auctionId, decimal amount);
    }
}
