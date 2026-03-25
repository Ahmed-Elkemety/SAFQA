using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard.AuctionRepo;
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
        public AuctionManager(IAuctionRepository auctionRepository) 
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
    }
}