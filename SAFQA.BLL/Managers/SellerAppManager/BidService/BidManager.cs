using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Bids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.BidService
{
    public class BidManager : IBidManager
    {
        private readonly IBidRepository _bidRepository;

        public BidManager(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public Task<int> GetBidsByCategory(string userId, int categoryId)
        {
            return _bidRepository.GetBidsByCategory(userId, categoryId);
        }

        public Task<int> GetSellerBids(string userId)
        {
            return _bidRepository.GetSellerBids(userId);
        }
    }
}