using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Category;
using SAFQA.DAL.Repository.Items;
using SAFQA.DAL.Repository.SellerDashboard.AuctionRepo;
using SAFQA.DAL.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.AuctionService
{
    public class AuctionManager : IAuctionManager
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IUserRepo _userRepo;
        public AuctionManager(IAuctionRepository auctionRepository, IitemsRepository itemsRepository, IcategoryRepo categoryRepository, IUserRepo userRepo)
        {
            _auctionRepository = auctionRepository;
            _userRepo = userRepo;
        }
        public Task<int> GetTotalSellerAuctions(int sellerId)
        {
            return _auctionRepository.CountAuctionsBySeller(sellerId);
        }

        public Task<int> GetActiveSellerAuctions(int sellerId)
        {
            return _auctionRepository.GetActiveSellerAuctions(sellerId);
        }

        public async Task<IEnumerable<TopWinnerDto>> GetWinnersBySeller(int sellerId)
        {
            var result = await _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId &&
                            a.WinnerUserId != null &&
                            !a.IsDeleted &&
                            a.Status == AuctionStatus.Finished)
                .Join(_userRepo.GetAll(),
                      a => a.WinnerUserId,
                      u => u.Id,
                      (a, u) => new { Auction = a, User = u })
                .GroupBy(x => new { x.User.Id, x.User.FullName, x.User.Email })
                .Select(g => new TopWinnerDto
                {
                    BuyerName = g.Key.FullName,
                    Email = g.Key.Email,
                    SellerCompanyName = g.Select(x => x.Auction.Seller.StoreName).FirstOrDefault(),
                    AuctionsWonCount = g.Count(),
                    TotalPaidAmount = g.Sum(x => x.Auction.FinalPrice + x.Auction.SecurityDeposit)
                })
                .OrderByDescending(x => x.TotalPaidAmount)
                .ToListAsync();

            return result.AsEnumerable(); 
        }

        public async Task<List<TopCustomerDto>> GetTopCustomers()
        {
            var data = await _auctionRepository.GetTopCustomersAsync();

            var result = data.Select(x => new TopCustomerDto
            {
                Name = x.Name,
                Email = x.Email,
                CompanyName = x.CompanyName,
                ParticipatedAuctions = x.ParticipatedAuctions,
                TotalPaid = x.TotalPaid
            }).ToList();

            return result;
        }

        public async Task<int> GetTotalAuctions()
        {
            return await _auctionRepository.GetTotalAuctionsCount();
        }
        public async Task<int> GetActiveAuctionsCount()
        {
            return await _auctionRepository.GetActiveAuctionsCount();
        }
        public async Task<int> GetExpiredAuctionsCount()
        {
            return await _auctionRepository.GetExpiredAuctionsCount();
        }
        public async Task<int> GetUpcomingAuctionsCount()
        {
            return await _auctionRepository.GetUpcomingAuctionsCount();
        }

        public async Task<List<AuctionProfitDto>> GetTopProfitableAuctions(int sellerId, int categoryId)
        {
            var validStatuses = new[] { AuctionStatus.Finished };

            var query = _auctionRepository.GetAll();

            var result = await query
                .Where(a =>
                    !a.IsDeleted &&
                    a.SellerId == sellerId &&
                    validStatuses.Contains(a.Status) &&
                    a.items.Any(i => i.CategoryId == categoryId)
                )
                .Select(a => new AuctionProfitDto
                {
                    Title = a.Title,
                    StartingPrice = a.StartingPrice,
                    FinalPrice = a.FinalPrice,
                    Profit = a.FinalPrice - a.StartingPrice,
                    WinnerName = a.AuctionUsers
                        .Where(au => au.UserId == a.WinnerUserId)
                        .Select(au => au.User.FullName)
                        .FirstOrDefault()
                })
                .OrderByDescending(a => a.FinalPrice - a.StartingPrice)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CategoryPercentageDto>> GetCategoryPercentageBySeller(int sellerId)
        {
            var auctions = _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId
                            && !a.IsDeleted
                            && (a.Status == AuctionStatus.Active || a.Status == AuctionStatus.Finished))
                .Include(a => a.items)
                .ThenInclude(i => i.Category);

            var allItems = auctions.SelectMany(a => a.items)
                                   .Where(i => i.CategoryId != null);

            var totalItems = allItems.Count();

            
            if (totalItems == 0)
                return new List<CategoryPercentageDto>();

            var result = allItems
                .GroupBy(i => i.Category.Name)
                .Select(g => new CategoryPercentageDto
                {
                    CategoryName = g.Key,
                    Percentage = Math.Round((double)g.Count() / totalItems * 100, 2) 
                })
                .OrderByDescending(c => c.Percentage)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<AuctionBidsDto>> GetSellerAuctionsBids(int sellerId)
        {
            var query = _auctionRepository.GetAll();

            var result = await query
                .Where(a =>
                    !a.IsDeleted &&
                    a.SellerId == sellerId &&
                    a.Bids.Any() 
                )
                .Select(a => new AuctionBidsDto
                {
                    Title = a.Title,
                    StartDate = a.StartDate,
                    TotalBids = a.Bids.Count()
                })
                .OrderByDescending(a => a.TotalBids)
                .ToListAsync();

            return result;
        }

        public IEnumerable<MonthlyEarningDto> GetMonthlyEarningsByCategory(int sellerId, int categoryId)
        {
            var auctions = _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId
                && a.items.Any(i => i.CategoryId == categoryId)
                && !a.IsDeleted)
                                .ToList();

            var grouped = auctions
                .GroupBy(a => new { a.EndDate.Year, a.EndDate.Month })
                .Select(g => new MonthlyEarningDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalEarnings = g.Sum(a => a.FinalPrice), // أو استخدم Transactions لو عايز دقة أكبر
                    AuctionCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return grouped;
        }

        public IEnumerable<PopularProductsDto> GetMostPopularProductsBySeller(int sellerId, int topCount = 5)
        {
            var sellerAuctions = _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId);

            // 2️⃣ نحسب عدد المشاهدات لكل منتج بناءً على Auction.ViewCount
            var productViews = sellerAuctions
                .SelectMany(a => a.items, (auction, item) => new PopularProductsDto
                {
                    Title = item.title,
                    Description = item.Description,
                    ViewCount = auction.ViewsCount // ✅ هنا بنستخدم عدد المشاهدات من المزاد
                })
                .OrderByDescending(p => p.ViewCount) // نرتب من الأكثر مشاهدة
                .Take(topCount) // نجيب فقط أعلى المنتجات
                .ToList();

            return productViews;
        }
    }
}