using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.RepoDtos.SellerApp.Home;

namespace SAFQA.DAL.Repository.Seller
{
    public class sellerRepo : IsellerRepo
    {
        private readonly SAFQA_Context _context;

        public sellerRepo(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task<SellerBasic?> GetSellerBasicAsync(string userId)
        {
            return await _context.Sellers
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .Select(s => new SellerBasic
                {
                    StoreName = s.StoreName,
                    StoreLogo = s.StoreLogo
                })
                .FirstOrDefaultAsync();
        }
    }
}
