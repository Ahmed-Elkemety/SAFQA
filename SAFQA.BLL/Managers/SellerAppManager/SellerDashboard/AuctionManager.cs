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
        public Task<int> GetTotalSellerAuctions(int sellerId)
        {
            return _auctionRepository.CountAuctionsBySeller(sellerId);
        }

        public Task<int> GetActiveSellerAuctions(int sellerId)
        {
            return _auctionRepository.GetActiveSellerAuctions(sellerId);
        }
    }
}
