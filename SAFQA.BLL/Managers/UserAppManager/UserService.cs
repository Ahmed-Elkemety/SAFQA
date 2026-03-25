using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Repository;
using SAFQA.DAL.Repository.AdminDashboard.Users;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager
{
    public class UserService : IUserService
    {
        private readonly IcategoryRepo _categoryRepo;
        private readonly IAuctionRepo _auctionRepo;
        private readonly IUserRepo _userRepo;

        public UserService(IcategoryRepo categoryRepo , IAuctionRepo auctionRepo, IUserRepo userRepo)
        {
            _categoryRepo = categoryRepo;
            _auctionRepo = auctionRepo;
            _userRepo = userRepo;
        }

        public async Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync()
        {
            // Repository ترجع المزادات المشهورة (Trending)
            var auctions = await _auctionRepo.GetTrendingAuctionsAsync(); // List<Auction>

            // تحويل Entity → Internal DTO
            var result = auctions.Select(a => new TrendingAuctionDto
            {
                Id = a.Id,
                Title = a.Title,
                Image = a.Image
            }).ToList();

            return result;
        }

        // 2️⃣ Categories with Details
        public async Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync()
        {
            // Repository ترجع Categories مع المزادات أو العناصر بداخلها
            var categories = await _categoryRepo.GetCategoriesWithDetailsAsync(); // List<Category>

            var result = categories.Select(c => new CategoryWithDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Image = c.Image,
                ItemCount = c.ItemCount
            }).ToList();

            return result;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepo.GetTotalUsers();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _userRepo.GetActiveUsersCount();
        }

        public async Task<int> GetBlockedUsersCountAsync()
        {
            return await _userRepo.GetBlockedUsersCount();
        }
    }
}