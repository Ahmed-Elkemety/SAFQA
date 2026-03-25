using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.AccountDto.Seller;
using SAFQA.BLL.Dtos.SellerAppDto.HomeDto;
using SAFQA.BLL.Managers.AccountManager.Auth;

namespace SAFQA.BLL.Managers.SellerAppManager
{
    public interface IsellerManager
    {
<<<<<<< HEAD
        Task<AuthResult> CreateSellerAsync(string userId, CreateSellerDto dto, string deviceId);
        Task<AuthResult> UploadPersonalDocsAsync(string userId, PersonalSellerDto dto);
        Task<AuthResult> UploadBusinessDocsAsync(string userId, BusinessSellerDto dto);

=======
        Task CreateSellerAsync(string userId, CreateSellerDto dto);
        Task<AuthResult> UploadPersonalDocsAsync(int sellerId, PersonalSellerDto dto);
        Task<AuthResult> UploadBusinessDocsAsync(int sellerId, BusinessSellerDto dto);
>>>>>>> a5e452654819aea897a02ba14612dd29b8d7e791
        Task<SellerBasicDto?> GetMySellerHomeAsync(string userId);
        Task<int> GetTotalSellersCount();
        Task<int> GetVerifiedSellersCount();
        Task<int> GetPendingSellersCount();
    }
}