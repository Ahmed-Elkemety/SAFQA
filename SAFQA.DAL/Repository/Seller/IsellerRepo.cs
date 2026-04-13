using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.RepoDtos.SellerApp.Bussiness_Account;
using SAFQA.DAL.RepoDtos.SellerApp.Home;

namespace SAFQA.DAL.Repository.Seller
{
    public interface IsellerRepo
    {
        IQueryable<Models.Seller> GetAll();
        Models.Seller GetById(string Id);
        void Update(Models.Seller seller);
        Task SaveChangesAsync();
        Task<SellerBasic?> GetSellerBasicAsync(string userId);
        Task<BusinessAccount?> GetBusinessAccountAsync(string userId);
        Models.Seller GetSellerDetails(string userId);
        Task<int> GetTotalSellersCount();
        Task<int> GetVerifiedSellersCount();
        Task<int> CountPendingSellers();
        Task<Models.Seller> GetByUserIdAsync(string userId);
    }
}
