using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard
{
    public class BidManager : IBidManager
    {
        private readonly IBidRepository _bidRepository;

        public BidManager(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public IQueryable<Bid> GetBidsByCategory(int sellerId, int categoryId)
        {
            return _bidRepository.GetBidsByCategory(sellerId, categoryId);
        }

        public IQueryable<Bid> GetSellerBids(int sellerId)
        {
            return _bidRepository.GetSellerBids(sellerId);
        }
    }
}
