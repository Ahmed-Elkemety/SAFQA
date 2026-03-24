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
                .GroupBy(x => new { x.User.Id, x.User.FullName, x.User.Email, x.seller.StoreName })
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

        public async Task<List<TopCustomerDto>> GetTopCustomersAsync()
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
    }
}