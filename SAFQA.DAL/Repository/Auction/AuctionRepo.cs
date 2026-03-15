using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.RepoDtos.UserApp.Home.TrendingAuction;

namespace SAFQA.DAL.Repository.Auction
{
    public class AuctionRepo : IAuctionRepo
    {
        private readonly SAFQA_Context _context;

        public AuctionRepo(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task<List<TrendingAuction>> GetTrendingAuctionsAsync()
        {
            return await _context.Auctions
                .Where(a => a.IsTrending && !a.IsDeleted)
                .OrderByDescending(a => a.ViewsCount)
                .Select(a => new TrendingAuction
                {
                    Id = a.Id,
                    Title = a.Title,
                    Image = a.Image
                })
                .ToListAsync();
        }

    }
}
