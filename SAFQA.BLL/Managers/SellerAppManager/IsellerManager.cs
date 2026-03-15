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
        Task CreateSellerAsync(string userId, CreateSellerDto dto);
        Task<AuthResult> UploadPersonalDocsAsync(int sellerId, PersonalSellerDto dto);
        Task<AuthResult> UploadBusinessDocsAsync(int sellerId, BusinessSellerDto dto);

        Task<SellerBasicDto?> GetMySellerHomeAsync(string userId);
    }
}
