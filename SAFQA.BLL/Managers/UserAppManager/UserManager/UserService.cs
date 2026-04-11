using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Repository;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Category;
using SAFQA.DAL.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SAFQA.BLL.Managers.UserAppManager.UserManager
{
    public class UserService : IUserService
    {
        private readonly IcategoryRepo _categoryRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IUserRepo _userRepo;

        public UserService(IcategoryRepo categoryRepo , IAuctionRepository auctionRepo, IUserRepo userRepo)
        {
            _categoryRepo = categoryRepo;
            _auctionRepo = auctionRepo;
            _userRepo = userRepo;
        }

        public async Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync()
        {
            var auctions = await _auctionRepo.GetTrendingAuctionsAsync(); // List<Auction>

            var result = auctions.Select(a => new TrendingAuctionDto
            {
                Id = a.Id,
                Title = a.Title,
                Image = a.Image
            }).ToList();

            return result;
        }

        public async Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync()
        {
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