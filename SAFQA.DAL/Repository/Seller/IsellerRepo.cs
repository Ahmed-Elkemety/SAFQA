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
        Task<SellerBasic?> GetSellerBasicAsync(string userId);
    }
}
