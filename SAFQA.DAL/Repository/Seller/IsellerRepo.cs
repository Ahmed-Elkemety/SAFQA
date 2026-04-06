using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.RepoDtos.SellerApp.Home;

namespace SAFQA.DAL.Repository.Seller
{
    public interface IsellerRepo
    {
        IQueryable<Models.Seller> GetAll();
        Models.Seller GetById(int Id);
        Task<SellerBasic?> GetSellerBasicAsync(string userId);
        Task<int> GetTotalSellersCount();
        Task<int> GetVerifiedSellersCount();
        Task<int> CountPendingSellers();
    }
}
