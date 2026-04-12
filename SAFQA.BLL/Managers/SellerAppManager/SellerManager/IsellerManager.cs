using SAFQA.BLL.Dtos.AccountDto.Seller;
using SAFQA.BLL.Dtos.SellerAppDto.BussinessAccountDto;
using SAFQA.BLL.Dtos.SellerAppDto.HomeDto;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerManager
{
    public interface IsellerManager
    {
        Task<AuthResult> CreateSellerAsync(string userId, CreateSellerDto dto, string deviceId);
        Task<AuthResult> UploadPersonalDocsAsync(string userId, PersonalSellerDto dto);
        Task<AuthResult> UploadBusinessDocsAsync(string userId, BusinessSellerDto dto);
        Task<SellerBasicDto?> GetMySellerHomeAsync(string userId);
        Task<BusinessAccountDto?> GetBusinessAccountAsync(string userId);
        Task<AuthResult> EditProfile(string userId, EditSellerProfileDto dto);
        Task<AuthResult> UpgradeSellerAsync(string userId, UpgradeType newUpgrade);
        Task<int> GetTotalSellersCount();
        Task<int> GetVerifiedSellersCount();
        Task<int> GetPendingSellersCount();
        PagedResult<PendingSellerDto> GetPendingSellers(int page, int pageSize);
        Task<bool> ApproveSeller(string userId);
        Task<bool> RejectSeller(string userId);
        PagedResult<SellerListDto> GetAllSellers(int page, int pageSize);
        Task<bool> SuspendSeller(string userId);
        Task<bool> RestoreSeller(string userId);
        SellerDetailsDto GetSellerDetails(string userId);
    }
}