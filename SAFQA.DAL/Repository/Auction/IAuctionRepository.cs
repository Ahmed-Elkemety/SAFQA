using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.RepoDtos.UserApp.Home.Categorys;
using SAFQA.DAL.RepoDtos.UserApp.Home.TrendingAuction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Auction
{
    public interface IAuctionRepository
    {
        IQueryable<Models.Auction> GetAll();
        Models.Auction GetById(int Id);
        Task<int> GetActiveSellerAuctions(int sellerId);
        Task<int> CountAuctionsBySeller(int sellerId);
        Task<List<(User User, Models.Seller Seller, Models.Auction AuctionDetails)>> GetSellerWinnersRawAsync(int sellerId);
        Task<List<(string UserId, string Name, string Email, string CompanyName, int ParticipatedAuctions, decimal TotalPaid)>> GetTopCustomersAsync();
        Task<int> GetTotalAuctionsCount();
        Task<int> GetActiveAuctionsCount();
        Task<int> GetExpiredAuctionsCount();
        Task<int> GetUpcomingAuctionsCount();

        Task<List<TrendingAuction>> GetTrendingAuctionsAsync();
        IQueryable<Models.Auction> GetSellerAuctions(string userId);
        void Add(Models.Auction auction);
        void Update(Models.Auction auction);
        void Delete(Models.Auction auction);
        Task AddAsync(Models.Auction auction);
        Task<Models.Auction?> GetByIdWithDetailsAsync(int auctionId);
        Task<Models.Auction?> GetByIdAsync(int id);
        Task<Models.Auction?> GetWithDetailsAsync(int id);
        Task<IEnumerable<Models.Auction>> GetAllAsync();

        Task<(List<Models.Auction>, int)> GetAuctionsByCategoryId(
                    int categoryId,
                    int pageNumber,
                    int pageSize,
                    List<AuctionStatus>? statuses,
                    List<int>? cityIds,
                    decimal? minPrice,
                    decimal? maxPrice,
                    AuctionSortBy sortBy,
                    int? userCityId);
        Task<(List<Models.Auction>, int)> GetFavoriteAuctions(string userId, int pageNumber, int pageSize, int? CategoryId, List<AuctionStatus>? statuses,
                    List<int>? cityIds,
                    decimal? minPrice,
                    decimal? maxPrice,
                    AuctionSortBy sortBy,
                    int? userCityId);

        Task<List<Models.Auction>> GetAllWithSellerAsync();

        Task<List<Models.Auction>> GetEndingSoonAsync(int page, int pageSize);
        Task<List<Models.Auction>> GetTrendingAsync(int page, int pageSize);

        Task SaveChangesAsync();
        Task<Models.Auction?> GetAuctionWithDeliveryAsync(int id);
        Task<List<Models.Auction>> SearchAsync(string query , int? CategoryId, List<AuctionStatus>? statuses,
                   List<int>? cityIds,
                   decimal? minPrice,
                   decimal? maxPrice,
                   AuctionSortBy sortBy,
                   int? userCityId);
        Task CreateAuctionParticipation(AuctionParticipations auctionParticipations);

    }
}