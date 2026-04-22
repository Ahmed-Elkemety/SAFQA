using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.UserAppDto.AuctionDto;
using SAFQA.DAL.Enums;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.BLL.Dtos.SellerAppDto.CategoryDto;

namespace SAFQA.BLL.Managers.UserAppManager.AuctionManager
{
    public class AuctionManagerU:IAuctionManagerU
    {
        private readonly SAFQA_Context _context;
        private readonly IAuctionRepository _auctionRepository;
        private readonly UserManager<User> _userManager;

        public AuctionManagerU(SAFQA_Context Context , IAuctionRepository auctionRepository , UserManager<User> userManager)
        {
            _context = Context;
            _auctionRepository = auctionRepository;
            _userManager = userManager;
        }

        public async Task<AuthResult> ReportAuctionAsync(string userId, CreateReportDto dto)
        {
            var exists = await _context.auctionReports
                .AnyAsync(r => r.AuctionId == dto.AuctionId && r.UserId == userId);

            if (exists)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "You already reported this auction"
                };
            }

            var report = new AuctionReport
            {
                AuctionId = dto.AuctionId,
                UserId = userId,
                Reason = dto.Reason,
            };

            await _context.auctionReports.AddAsync(report);
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Report submitted successfully"
            };
        }
        public async Task<(AuthResult, object)> GetAuctionsByCategory(
    int categoryId,
    string userId,
    int pageNumber,
    int pageSize,
    AuctionQueryDto queryDto)
        {
            queryDto ??= new AuctionQueryDto();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                }, null);
            }

            var (auctions, totalCount) = await _auctionRepository
                .GetAuctionsByCategoryId(
                    categoryId,
                    pageNumber,
                    pageSize,
                    queryDto.Statuses,
                    queryDto.CityIds,
                    queryDto.MinPrice,
                    queryDto.MaxPrice,
                    queryDto.SortBy,
                    user.CityId);

            if (!auctions.Any())
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "No auctions found for this category"
                }, null);

            var auctionsDto = auctions.Select(a => new CategoryDto
            {
                AuctionId = a.Id,
                Title = a.Title,
                TotalBids = a.TotalBids,
                Status = a.Status,

                DisplayPrice =
                    a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled
                        ? a.StartingPrice
                        : a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon
                            ? a.CurrentPrice
                            : a.Status == AuctionStatus.Finished
                                ? a.FinalPrice
                                : 0,

                DisplayDate =
                    a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled
                        ? a.StartDate
                        : a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon
                            ? a.EndDate
                            : a.Status == AuctionStatus.Finished
                                ? a.EndDate
                                : DateTime.MinValue,

                Image = a.Image
            }).ToList();

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Auctions retrieved successfully",
            },
            new
            {
                Data = auctionsDto,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        public async Task<(AuthResult, List<FavoritesDto>, int)> GetFavoriteAuctions(
    string userId, int pageNumber, int pageSize , AuctionQueryDto queryDto)
        {
            queryDto ??= new AuctionQueryDto();
            var user = await _userManager.FindByIdAsync(userId);

            var (auctions, totalCount) =
                await _auctionRepository.GetFavoriteAuctions(userId, pageNumber, pageSize,
                    queryDto.CategoryId,
                    queryDto.Statuses,
                    queryDto.CityIds,
                    queryDto.MinPrice,
                    queryDto.MaxPrice,
                    queryDto.SortBy,
                    user.CityId);

            var data = auctions.Select(a => new FavoritesDto
            {
                AuctionId = a.Id,
                Title = a.Title,

                DisplayPrice =
                    a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled
                        ? a.StartingPrice
                        : a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon
                            ? a.CurrentPrice
                            : a.Status == AuctionStatus.Finished
                                ? a.FinalPrice
                                : a.CurrentPrice,

                DisplayDate =
                    a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled
                        ? a.StartDate
                        : a.EndDate,

                TotalBids = a.TotalBids,
                Status = a.Status,
                Image = a.Image
            }).ToList();

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Favorite auctions retrieved successfully"
            }, data, totalCount);
        }
        public async Task CalculateHotScoresAsync()
        {
            var auctions = await _auctionRepository.GetAllWithSellerAsync();

            var rawScores = new List<(Auction auction, double score)>();

            foreach (var a in auctions)
            {
                // ✅ استبعاد الحالات المنتهية أو الملغية
                if (a.Status == AuctionStatus.Finished || a.Status == AuctionStatus.Cancelled)
                {
                    a.HotScore = 0;
                    a.IsTrending = false;
                    continue;
                }

                double upgradeBoost = a.Seller.upgradeType switch
                {
                    UpgradeType.Elite => 0.7,
                    UpgradeType.Premium => 0.4,
                    UpgradeType.Basic => 0.2,
                    _ => 0
                };

                double priceGrowth = 0;
                double bidScore = 0;
                double ParticipationBoost = Math.Log(1 + a.ParticipationCount) * 0.54;



                // 🟢 حالة 2 و 3 (مزاد شغال)
                if (a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon)
                {
                    priceGrowth = a.StartingPrice == 0 ? 0 :
                        (double)(a.CurrentPrice - a.StartingPrice) / (double)a.StartingPrice;

                    bidScore = Math.Log(1 + a.TotalBids) * 0.5;
                    ParticipationBoost = 0;
                }

                // 🟡 حالة 1 (مزاد جديد)
                else if (a.Status == AuctionStatus.Upcoming)
                {
                    priceGrowth = 0;
                    bidScore = 0;
                }

                double rawScore =
                    (Math.Log(1 + a.ViewsCount) * 0.2) +
                    (Math.Log(1 + a.LikesCount) * 0.3) +
                    bidScore +
                    (priceGrowth * 0.3) +
                    ((a.Seller.Rating / 5.0) * 0.4) +
                    upgradeBoost+
                    ParticipationBoost;

                rawScores.Add((a, rawScore));
            }

            // ✅ Normalize من 1 → 10
            var min = rawScores.Min(x => x.score);
            var max = rawScores.Max(x => x.score);

            foreach (var item in rawScores)
            {
                double normalized = (max - min) == 0 ? 1 :
                    1 + ((item.score - min) / (max - min)) * 9;

                item.auction.HotScore = normalized;
                item.auction.IsTrending = normalized >= 7;
            }

            await _auctionRepository.SaveChangesAsync();
        }
    }
}
