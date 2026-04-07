using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto;
using SAFQA.BLL.Enums;
using static SAFQA.BLL.Help.Helper;

namespace SAFQA.BLL.Managers.SellerAppManager.AuctionManager
{
    public interface IAuctionService
    {
        Task<PagedResult<SellerActionHistoryDto>> GetHistory(
            string userId,
            AuctionStatus? status,
            int page,
            int pageSize);
    }
}
