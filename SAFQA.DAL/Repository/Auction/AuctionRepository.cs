using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.RepoDtos.UserApp.Home.TrendingAuction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.RepoDtos.UserApp.Home.Categorys;

namespace SAFQA.DAL.Repository.Auction
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly SAFQA_Context _context;

        public AuctionRepository(SAFQA_Context context)
        {
            _context = context;
        }

        public Task<int> CountAuctionsBySeller(int sellerId)
        {
            return _context.Auctions
                    .Where(a => a.SellerId == sellerId)
                    .CountAsync();
        }

        public Task<int> GetActiveSellerAuctions(int sellerId)
        {
            return _context.Auctions
                            .Where(a => a.SellerId == sellerId && a.Status == AuctionStatus.Active)
                            .CountAsync();
        }


        public async Task<List<(User User, Models.Seller Seller, Models.Auction AuctionDetails)>> GetSellerWinnersRawAsync(int sellerId)
        {
            var query = await _context.Auctions
        .Where(a => a.SellerId == sellerId &&
                    !string.IsNullOrEmpty(a.WinnerUserId) &&
                    a.Status == AuctionStatus.Finished)
        .Join(
            _context.Users,
            a => a.WinnerUserId,
            u => u.Id,
            (a, u) => new { Auction = a, User = u }
        )
        .Where(x => x.Auction != null && x.User != null && x.Auction.Seller != null)
        .ToListAsync();

            return query.Select(x => (x.User, x.Auction.Seller, AuctionDetails: x.Auction)).ToList();
        }


        public async Task<List<(string UserId, string Name, string Email, string CompanyName, int ParticipatedAuctions, decimal TotalPaid)>> GetTopCustomersAsync()
        {
            var result = await _context.AuctionUsers
                .Where(au =>
                    au.Auction.Status == AuctionStatus.Active ||
                    au.Auction.Status == AuctionStatus.Finished)

                .GroupBy(au => new
                {
                    au.User.Id,
                    au.User.FullName,
                    au.User.Email,
                    CompanyName = au.User.Seller != null ? au.User.Seller.StoreName : "Customer"
                })

                .Select(g => new ValueTuple<string, string, string, string, int, decimal>(
                    g.Key.Id,
                    g.Key.FullName,
                    g.Key.Email,
                    g.Key.CompanyName,

                    g.Select(x => x.AuctionId).Distinct().Count(),
                    
                    g.Sum(x =>
                        x.Auction.SecurityDeposit +
                        (x.Auction.WinnerUserId == x.UserId ? x.Auction.FinalPrice : 0) 
                    )
                ))

                .OrderByDescending(x => x.Item5)
                .Take(5)

                .ToListAsync();

            return result;
        }

        public async Task<int> GetTotalAuctionsCount()
        {
            return await _context.Auctions
                                 .Where(a => !a.IsDeleted)
                                 .CountAsync();
        }
        public async Task<int> GetActiveAuctionsCount()
        {
            return await _context.Auctions
                .Where(a => !a.IsDeleted
                            && (a.Status == AuctionStatus.Active
                                || a.Status == AuctionStatus.EndingSoon))
                .CountAsync();
        }
        public async Task<int> GetExpiredAuctionsCount()
        {
            DateTime now = DateTime.UtcNow;

            return await _context.Auctions
                .Where(a => !a.IsDeleted
                            && (
                                a.Status == AuctionStatus.Finished
                                || a.Status == AuctionStatus.Cancelled
                                || a.EndDate < now
                               ))
                .CountAsync();
        }

        public async Task<int> GetUpcomingAuctionsCount()
        {
            DateTime now = DateTime.UtcNow;

            return await _context.Auctions
                .Where(a => !a.IsDeleted
                            && a.Status == AuctionStatus.Upcoming &&
                            a.StartDate > now)
                .CountAsync();
        }

        public async Task<List<TrendingAuction>> GetTrendingAuctionsAsync()
        {
            return await _context.Auctions
                .Where(a => a.IsTrending && !a.IsDeleted)
                .OrderByDescending(a => a.ViewsCount)
                .Select(a => new TrendingAuction
                {
                    Id = a.Id,
                    Title = a.Title,
                    Image = a.Image
                })
                .ToListAsync();
        }

        public IQueryable<Models.Auction> GetSellerAuctions(string userId)
        {
            return _context.Auctions
                .AsNoTracking()
                .Where(a => a.Seller.UserId == userId && a.IsDeleted == false);
        }


        public IQueryable<Models.Auction> GetAll()
        {
            return _context.Auctions;
        }


        public Models.Auction GetById(int Id)
        {
            return _context.Auctions.FirstOrDefault(s => s.Id == Id);
        }


        public void Add(Models.Auction auction)
        {
            _context.Auctions.Add(auction);
            _context.SaveChanges();
        }

        public void Update(Models.Auction auction)
        {
            _context.Auctions.Update(auction);
            _context.SaveChanges();
        }

        public void Delete(Models.Auction auction)
        {
            _context.Auctions.Remove(auction);
            _context.SaveChanges();
        }

        public async Task AddAsync(Models.Auction auction)
        {
            await _context.Auctions.AddAsync(auction);
            _context.SaveChanges();
        }

        public async Task<Models.Auction?> GetByIdWithDetailsAsync(int auctionId)
        {
            return await _context.Auctions
                .Include(a => a.Seller)
                .Include(a => a.items)
                    .ThenInclude(i => i.images)
                .Include(a => a.items)
                    .ThenInclude(i => i.itemAttributesValues)
                .FirstOrDefaultAsync(a => a.Id == auctionId && !a.IsDeleted);
        }

        public async Task<Models.Auction?> GetByIdAsync(int id)
        {
            return await _context.Auctions
            .AsNoTracking()
            .Where(a => a.Id == id && !a.IsDeleted)
            .Include(a => a.Seller)
            .Include(a => a.items)
                .ThenInclude(i => i.images)
            .Include(a => a.items)
                .ThenInclude(i => i.itemAttributesValues)
                    .ThenInclude(av => av.categoryAttributes)
            .FirstOrDefaultAsync();
        }

        public async Task<Models.Auction?> DeliveryByIdAsync(int id)
        {
            return await _context.Auctions
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<Models.Auction?> GetAuctionWithDeliveryAsync(int id)
        {
            return await _context.Auctions
                .Include(a => a.delivery)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<Models.Auction?> GetWithDetailsAsync(int id)
        {
            return await _context.Auctions
                .Include(a => a.items)
                    .ThenInclude(i => i.images)
                .Include(a => a.items)
                    .ThenInclude(i => i.itemAttributesValues)
                        .ThenInclude(v => v.categoryAttributes)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<IEnumerable<Models.Auction>> GetAllAsync()
        {
            return await _context.Auctions
            .AsNoTracking()
            .Where(a => !a.IsDeleted)
            .Include(a => a.Seller)
            .Include(a => a.items)
                .ThenInclude(i => i.images)
            .Include(a => a.items)
                .ThenInclude(i => i.itemAttributesValues)
                    .ThenInclude(av => av.categoryAttributes)
            .ToListAsync();
        }

        public async Task<(List<Models.Auction>, int)> GetAuctionsByCategoryId(
                   int categoryId,
                   int pageNumber,
                   int pageSize,
                   List<AuctionStatus>? statuses,
                   List<int>? cityIds,
                   decimal? minPrice,
                   decimal? maxPrice,
                   AuctionSortBy sortBy,
                   int? userCityId)
        {
            var query = _context.Auctions
                .Where(a => !a.IsDeleted && a.CategoryId == categoryId);

            if (statuses is { Count: > 0 })
            {
                query = query.Where(a => statuses.Contains(a.Status));
            }

            if (cityIds is { Count: > 0 })
            {
                query = query.Where(a => a.Seller != null && cityIds.Contains(a.Seller.CityId));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(a => (a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice) >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(a => (a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice) <= maxPrice.Value);
            }

            query = sortBy switch
            {
                AuctionSortBy.MostBids => query.OrderByDescending(a => a.TotalBids),
                AuctionSortBy.Nearest when userCityId.HasValue => query
                    .OrderByDescending(a => a.Seller != null && a.Seller.CityId == userCityId.Value)
                    .ThenByDescending(a => a.CreatedAt),
                AuctionSortBy.PriceHighToLow => query.OrderByDescending(a => a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice),
                AuctionSortBy.PriceLowToHigh => query.OrderBy(a => a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice),
                _ => query.OrderByDescending(a => a.CreatedAt)
            };


            var totalCount = await query.CountAsync();

            var auctions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (auctions, totalCount);
        }

        public async Task<(List<Models.Auction>, int)> GetFavoriteAuctions(string userId, int pageNumber, int pageSize, int? CategoryId, List<AuctionStatus>? statuses,
                   List<int>? cityIds,
                   decimal? minPrice,
                   decimal? maxPrice,
                   AuctionSortBy sortBy,
                   int? userCityId)
        {
            var query = _context.auctionLikes
                .Where(al => al.UserId == userId && !al.Auction.IsDeleted)
                .Include(al => al.Auction)
                .Select(al => al.Auction)
                .AsNoTracking();

            if (CategoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == CategoryId.Value);
            }

            if (statuses is { Count: > 0 })
            {
                query = query.Where(a => statuses.Contains(a.Status));
            }

            if (cityIds is { Count: > 0 })
            {
                query = query.Where(a => a.Seller != null && cityIds.Contains(a.Seller.CityId));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(a => (a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice) >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(a => (a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice) <= maxPrice.Value);
            }

            query = sortBy switch
            {
                AuctionSortBy.MostBids => query.OrderByDescending(a => a.TotalBids),
                AuctionSortBy.Nearest when userCityId.HasValue => query
                    .OrderByDescending(a => a.Seller != null && a.Seller.CityId == userCityId.Value)
                    .ThenByDescending(a => a.CreatedAt),
                AuctionSortBy.PriceHighToLow => query.OrderByDescending(a => a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice),
                AuctionSortBy.PriceLowToHigh => query.OrderBy(a => a.Status == AuctionStatus.Finished ? a.FinalPrice : a.CurrentPrice),
                _ => query.OrderByDescending(a => a.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<List<Models.Auction>> GetAllWithSellerAsync()
        {
            return await _context.Auctions
                .Include(a => a.Seller)
                .ToListAsync();
        }

        public async Task<List<Models.Auction>> GetEndingSoonAsync(int page, int pageSize)
        {
            return await _context.Auctions
                .Where(a => !a.IsDeleted && a.Status == AuctionStatus.EndingSoon)
                .OrderBy(a => a.EndDate)
                .ThenByDescending(a => a.HotScore)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Models.Auction>> GetTrendingAsync(int page, int pageSize)
        {
            return await _context.Auctions
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.HotScore)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Models.Auction>> SearchAsync(string query, int? CategoryId, List<AuctionStatus>? statuses,
                   List<int>? cityIds,
                   decimal? minPrice,
                   decimal? maxPrice,
                   AuctionSortBy sortBy,
                   int? userCityId)
        {
            var auctions = _context.Auctions
        .Where(a => !a.IsDeleted && a.Status < AuctionStatus.Finished);

            // 🔍 Search
            if (!string.IsNullOrWhiteSpace(query))
            {
                if (int.TryParse(query, out int id))
                {
                    auctions = auctions.Where(a => a.Id == id);
                }
                else
                {
                    auctions = auctions.Where(a =>
                        EF.Functions.Like(a.Title, $"%{query}%"));
                }
            }

            // 📂 Category
            if (CategoryId.HasValue)
            {
                auctions = auctions.Where(a => a.CategoryId == CategoryId.Value);
            }

            // 🟢 Status
            if (statuses is { Count: > 0 })
            {
                auctions = auctions.Where(a => statuses.Contains(a.Status));
            }

            // 📍 City
            if (cityIds is { Count: > 0 })
            {
                auctions = auctions.Where(a =>
                    a.Seller != null && cityIds.Contains(a.Seller.CityId));
            }

            // 💰 Price (مهم: نحسب مرة واحدة)
            auctions = auctions.Select(a => new
            {
                Auction = a,
                Price = a.Status == AuctionStatus.Upcoming
                    ? a.StartingPrice
                    : a.Status == AuctionStatus.Finished
                        ? a.FinalPrice
                        : a.CurrentPrice
            })
            .Where(x =>
                (!minPrice.HasValue || x.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || x.Price <= maxPrice.Value)
            )
            .Select(x => x.Auction);

            // 🔽 Sorting
            auctions = sortBy switch
            {
                AuctionSortBy.MostBids =>
                    auctions.OrderByDescending(a => a.TotalBids),

                AuctionSortBy.Nearest when userCityId.HasValue =>
                    auctions
                        .OrderByDescending(a => a.Seller != null && a.Seller.CityId == userCityId)
                        .ThenByDescending(a => a.CreatedAt),

                AuctionSortBy.PriceHighToLow =>
                    auctions.OrderByDescending(a =>
                        a.Status == AuctionStatus.Upcoming
                            ? a.StartingPrice
                            : a.Status == AuctionStatus.Finished
                                ? a.FinalPrice
                                : a.CurrentPrice),

                AuctionSortBy.PriceLowToHigh =>
                    auctions.OrderBy(a =>
                        a.Status == AuctionStatus.Upcoming
                            ? a.StartingPrice
                            : a.Status == AuctionStatus.Finished
                                ? a.FinalPrice
                                : a.CurrentPrice),

                _ =>
                    auctions
                        .OrderByDescending(a => a.IsFeatured)
                        .ThenByDescending(a => a.CreatedAt)
            };

            // ⚡ Limit + Execute
            return await auctions
                .Take(20)
                .ToListAsync();
        }
        

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}