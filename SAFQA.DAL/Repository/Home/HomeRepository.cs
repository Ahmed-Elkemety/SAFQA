using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Dtos.UserAppDto.HomeDto;

namespace SAFQA.DAL.Repository.Home
{
    public class HomeRepository : IHomeRepository
    {
        private readonly SAFQA_Context _context;

        public HomeRepository(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync()
        {
            return await _context.Auctions
                .Where(a => a.IsTrending && !a.IsDeleted)
                .OrderByDescending(a => a.ViewsCount)
                .Select(a => new TrendingAuctionDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Image = a.Image
                })
                .ToListAsync();
        }

        public async Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync()
        {
            return await _context.Category
                .Select(c => new CategoryWithDetailsDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = c.Image,
                    ItemCount = c.Items.Count()
                })
                .ToListAsync();
        }
    }
}
