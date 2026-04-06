using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard.AuctionRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Repository.SellerDashboard.ItemRepo;
using SAFQA.DAL.Repository.Category;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.AuctionService
{
    public class AuctionManager : IAuctionManager
    {
        private readonly IAuctionRepository _auctionRepository;
        public AuctionManager(IAuctionRepository auctionRepository, IitemsRepository itemsRepository, IcategoryRepo categoryRepository)
        {
            _auctionRepository = auctionRepository;
        }
        public Task<int> GetTotalSellerAuctions(int sellerId)
        {
            return _auctionRepository.CountAuctionsBySeller(sellerId);
        }

        public Task<int> GetActiveSellerAuctions(int sellerId)
        {
            return _auctionRepository.GetActiveSellerAuctions(sellerId);
        }

        public async Task<List<SellerWinnerDto>> GetSellerWinnersAsync(int sellerId)
        {
            var rawData = await _auctionRepository.GetSellerWinnersRawAsync(sellerId);

            var dtoList = rawData
                .GroupBy(x => new { x.User.Id, x.User.FullName, x.User.Email, x.Seller.StoreName })
                .Select(g => new SellerWinnerDto
                {
                    UserFullName = g.Key.FullName,
                    UserEmail = g.Key.Email,
                    SellerStoreName = g.Key.StoreName,
                    WonAuctionsCount = g.Count(),
                    TotalNetProfit = g.Sum(x => x.AuctionDetails.FinalPrice - x.AuctionDetails.StartingPrice)
                })
                .ToList();

            return dtoList;
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
            var validStatuses = new[] { AuctionStatus.Finished, AuctionStatus.Active, AuctionStatus.EndingSoon };

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
                    Percentage = Math.Round((double)g.Count() / totalItems * 100, 2) // نسبة مئوية مع تقريب ل2 منازل
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
    }
}