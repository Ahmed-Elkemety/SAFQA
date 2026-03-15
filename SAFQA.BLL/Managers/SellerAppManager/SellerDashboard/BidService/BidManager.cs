using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard.BidRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.BidService
{
    public class BidManager : IBidManager
    {
        private readonly IBidRepository _bidRepository;

        public BidManager(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public Task<int> GetBidsByCategory(int sellerId, int categoryId)
        {
            return _bidRepository.GetBidsByCategory(sellerId, categoryId);
        }

        public Task<int> GetSellerBids(int sellerId)
        {
            return _bidRepository.GetSellerBids(sellerId);
        }

        public async Task<List<AuctionBidsDto>> GetTop4AuctionsBySeller(int sellerId)
        {
            var rawData = await _bidRepository.GetAuctionsWithBidsRawBySellerAsync(sellerId);

            var topAuctions = rawData
                .Select(a => new AuctionBidsDto
                {
                    AuctionId = a.AuctionId,
                    AuctionTitle = a.AuctionTitle,
                    ProductNames = a.ProductNames,
                    TotalBids = a.TotalBids
                })
                .OrderByDescending(a => a.TotalBids) 
                .Take(4) 
                .ToList();

            return topAuctions;
        }
    }
}
