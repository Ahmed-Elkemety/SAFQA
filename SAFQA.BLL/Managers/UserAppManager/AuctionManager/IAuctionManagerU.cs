using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.UserAppDto.AuctionDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.UserAppManager.AuctionManager
{
    public interface IAuctionManagerU
    {
        Task<AuthResult> ReportAuctionAsync(string userId, CreateReportDto dto);
        Task<(AuthResult, object)> GetAuctionsByCategory(int categoryId, string userId, int pageNumber, int pageSize);
    }
}
