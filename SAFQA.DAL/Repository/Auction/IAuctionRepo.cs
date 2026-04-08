using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.RepoDtos.UserApp.Home.TrendingAuction;

namespace SAFQA.DAL.Repository.Auction
{
    public interface IAuctionRepo
    {
        Task<List<TrendingAuction>> GetTrendingAuctionsAsync();
        IQueryable<Models.Auction> GetSellerAuctions(string userId);

        IQueryable<Models.Auction> GetAll();
        Models.Auction GetById(int id);
        void Add(Models.Auction auction);
        void Update(Models.Auction auction);
        void Delete(Models.Auction auction);

    }
}
