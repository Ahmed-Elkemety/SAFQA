using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard
{
    public class AuctionManager : IAuctionManager
    {
        private readonly IAuctionRepository _auctionRepository;
        public AuctionManager(IAuctionRepository auctionRepository) 
        {
            _auctionRepository = auctionRepository;
        }

        public IQueryable<Auction> GetAllAuctions(int sellerId)
        {
            return _auctionRepository.GetSellerAuctions(sellerId);
        }

        public Task<int> GetTotalAuctionsAsync(int sellerId)
        {
            return _auctionRepository.CountAuctionsBySellerAsync(sellerId);
        }

        public IQueryable<Auction> GetActiveAuctions(int sellerId)
        {
            return _auctionRepository.GetActiveSellerAuctions(sellerId);
        }
    }
}
