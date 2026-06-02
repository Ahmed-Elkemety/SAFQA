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
using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto.AuctionProcessDtos;
using SAFQA.DAL.Repository.Wallet;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using SAFQA.DAL.Repository.Category;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SAFQA.BLL.Managers.SellerAppManager.Notification;
using SAFQA.BLL.Managers.UserAppManager.NotificationService;

namespace SAFQA.BLL.Managers.UserAppManager.AuctionManager
{
    public class AuctionManagerU : IAuctionManagerU
    {
        private readonly SAFQA_Context _context;
        private readonly IAuctionRepository _auctionRepository;
        private readonly UserManager<User> _userManager;
        private readonly IWalletRepo _wallet;
        private readonly IcategoryRepo _category;
        private readonly INotificationService _notification;

        public AuctionManagerU(SAFQA_Context Context , IAuctionRepository auctionRepository , UserManager<User> userManager , IWalletRepo wallet,IcategoryRepo category,INotificationService notification)
        {
            _context = Context;
            _auctionRepository = auctionRepository;
            _userManager = userManager;
            _wallet = wallet;
            _category = category;
            _notification = notification;
        }


        public async Task<AuthResult> ReportAuctionAsync(string userId, CreateReportDto dto)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

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
                CreatedAt = DateTime.UtcNow
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

            var auctionsDto = auctions.Select(a => new Dtos.UserAppDto.AuctionDto.CategoryDto
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



                if (a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon)
                {
                    priceGrowth = a.StartingPrice == 0 ? 0 :
                        (double)(a.CurrentPrice - a.StartingPrice) / (double)a.StartingPrice;

                    bidScore = Math.Log(1 + a.TotalBids) * 0.5;
                    ParticipationBoost = 0;
                }

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

        public async Task<(AuthResult, AuctionDetailsDto?)> GetAuctionDetails(int auctionId, string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                }, null);
            }
            var auction = await _auctionRepository.GetByIdAsync(auctionId);

            if (auction == null)
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "Auction not found"
                }, null);
            }

            if(auction.Seller.UserId == userId)
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "The Seller Can Not Be Participaion In Auction"
                }, null);
            }

            var  userfaviorate = _context.auctionLikes
                .Any(l => l.AuctionId == auctionId && l.UserId == userId);
            bool ok;
            if (userfaviorate)
            {
                ok = true;
            }
            else
            {
                ok = false;
            }

            var dto = new AuctionDetailsDto
            {
                IsFaviorate = ok,
                Id = auction.Id,
                Title = auction.Title,
                Description = auction.Description,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate,

                CurrentPrice = auction.Status == AuctionStatus.Upcoming
                                ? auction.StartingPrice
                                : auction.Status == AuctionStatus.Finished
                                    ? auction.FinalPrice
                                    : auction.CurrentPrice,
                TotalBids = auction.TotalBids,

                SecurityDeposit = auction.SecurityDeposit,
                BidIncrement = auction.BidIncrement,

                SellerId = auction.SellerId ?? 0,
                StoreName = auction.Seller?.StoreName ?? "",
                StoreLogo = auction.Seller?.StoreLogo != null
                    ? Convert.ToBase64String(auction.Seller.StoreLogo)
                    : null,

                Items = auction.items.Select(i => new ItemDto
                {
                    Id = i.Id,
                    Title = i.title,
                    Count = i.Count,
                    Description = i.Description,
                    Condition = i.Condition.ToString(),
                    WarrantyInfo = i.WarrantyInfo,

                    Images = i.images
                        .Select(img => Convert.ToBase64String(img.Image))
                        .ToList(),

                    Attributes = i.itemAttributesValues
                        .Select(attr => new ItemAttributeDto
                        {
                            Id = attr.Id,
                            AttributeName = attr.categoryAttributes?.Name ?? "",
                            Image = attr.categoryAttributes.Image,
                            Value = attr.value
                        }).ToList()

                }).ToList()
            };

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Auction retrieved successfully"
            }, dto);
        }

        public async Task<AuthResult> CheckSecurityDeposit(int auctionId, string userId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);

            if (auction == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Auction not found"
                };
            }

            var wallet = await _wallet.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Wallet not found"
                };
            }

            if (wallet.Balance < auction.SecurityDeposit)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Insufficient balance for security deposit"
                };
            }

            var exists = await _context.auctionParticipations
                .AnyAsync(x =>
                    x.AuctionId == auctionId &&
                    x.UserId == userId);

            if (exists)
            {
                return new AuthResult
                {
                    IsSuccess = true,
                    Message = "You have already joined this auction"
                };
            }

            try
            {
                var auctionParticipation = new AuctionParticipations
                {
                    AuctionId = auctionId,
                    UserId = userId,
                    PatoicipationTime = DateTime.UtcNow
                };

                auction.ParticipationCount++;

                await _auctionRepository.CreateAuctionParticipation(auctionParticipation);
                await _auctionRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                      && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                return new AuthResult
                {
                    IsSuccess = true,
                    Message = "You have already joined this auction"
                };
            }

            return new AuthResult
            {
                IsSuccess = true,
                Message = "You have enough balance to proceed"
            };
        }

        public async Task UpdateAuctionStatusesAsync()
        {
            try
            {
                var now = DateTime.UtcNow;

                // 1. بيانات خفيفة
                var auctionsData = await _context.Auctions
                    .Where(a => a.Status != AuctionStatus.Finished)
                    .Select(a => new
                    {
                        a.Id,
                        a.StartDate,
                        a.EndDate,
                        a.Status,
                        a.SellerId,
                        SellerUserId = a.Seller.UserId
                    })
                    .ToListAsync();

                if (!auctionsData.Any())
                    return;

                var auctionIds = auctionsData.Select(a => a.Id).ToList();

                // 2. tracked Auctions
                var trackedAuctions = await _context.Auctions
                    .Where(a => auctionIds.Contains(a.Id))
                    .ToDictionaryAsync(a => a.Id);

                // 3. آخر Bid لكل Auction
                var lastBids = await _context.Bids
                    .Where(b => b.AuctionId.HasValue && auctionIds.Contains(b.AuctionId.Value))
                    .GroupBy(b => b.AuctionId)
                    .Select(g => g.OrderByDescending(b => b.Date).FirstOrDefault())
                    .ToListAsync();

                // بدل FirstOrDefault داخل loop → Dictionary أسرع
                var lastBidsDict = lastBids
                    .Where(b => b != null && b.AuctionId.HasValue)
                    .ToDictionary(b => b.AuctionId!.Value, b => b);

                // 4. كل UserIds الخاصة بالـ Wallets
                var userIds = lastBids
                    .Where(b => b != null && !string.IsNullOrEmpty(b.UserId))
                    .Select(b => b.UserId)
                    .Concat(auctionsData.Select(a => a.SellerUserId))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .ToList();

                // 5. Wallets
                var wallets = await _context.Wallets
                    .Where(w => userIds.Contains(w.UserId))
                    .ToDictionaryAsync(w => w.UserId);

                // 6. المشاركين في كل Auction
                var participations = await _context.auctionParticipations
                    .Where(p => auctionIds.Contains(p.AuctionId))
                    .GroupBy(p => p.AuctionId)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.Select(x => x.UserId).Distinct().ToList()
                    );

                // 7. Delivery الموجودة بالفعل
                var existingDeliveries = (await _context.Delivery
                    .Where(d => auctionIds.Contains(d.AuctionId))
                    .Select(d => d.AuctionId)
                    .ToListAsync())
                    .ToHashSet();

                // تجميع الإشعارات بدل إرسالها داخل loop
                var pendingStatusNotifications =
                    new List<(int auctionId, string status, List<string> users)>();

                var pendingFinishedNotifications =
                    new List<(int auctionId, decimal finalPrice, string winnerUserId, List<string> users)>();

                // 8. Loop
                foreach (var a in auctionsData)
                {
                    if (!trackedAuctions.TryGetValue(a.Id, out var auction))
                        continue;

                    var oldStatus = auction.Status;

                    if (now < a.StartDate)
                    {
                        auction.Status = AuctionStatus.Upcoming;
                    }
                    else if (now >= a.EndDate)
                    {
                        auction.Status = AuctionStatus.Finished;
                        auction.UpdatedAt = now;

                        lastBidsDict.TryGetValue(a.Id, out var lastBid);

                        if (lastBid != null && !string.IsNullOrEmpty(lastBid.UserId))
                        {
                            auction.WinnerUserId = lastBid.UserId;
                            auction.FinalPrice = lastBid.Amount;

                            // Delivery
                            if (a.SellerId != null && !existingDeliveries.Contains(a.Id))
                            {
                                _context.Delivery.Add(new Delivery
                                {
                                    Code = GenerateRandomCode(),
                                    Status = DeliveryStatus.Orderplaced,
                                    AuctionId = a.Id,
                                    SellerId = a.SellerId.Value,
                                    UserId = lastBid.UserId
                                });
                            }

                            // Wallets + حماية من negative balance
                            if (wallets.TryGetValue(lastBid.UserId, out var buyerWallet) &&
                                wallets.TryGetValue(a.SellerUserId, out var sellerWallet))
                            {
                                if (buyerWallet.Balance >= lastBid.Amount)
                                {
                                    var before = buyerWallet.Balance;

                                    buyerWallet.Balance -= lastBid.Amount;
                                    buyerWallet.FrozenBalance += lastBid.Amount;

                                    _context.Transactions.Add(new Transactions
                                    {
                                        Type = TransactionType.Purchase,
                                        Status = TransactionStatus.Completed,
                                        WalletId = buyerWallet.Id,
                                        Amount = lastBid.Amount,
                                        BalanceBefore = before,
                                        BalanceAfter = buyerWallet.Balance,
                                        Description = $"Payment for Auction #{a.Id}",
                                        CreatedAt = now
                                    });
                                }
                            }
                        }

                        // تجميع Finished Notification فقط
                        if (participations.TryGetValue(a.Id, out var users))
                        {
                            var usersToNotify = users
                                .Append(a.SellerUserId)
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Distinct()
                                .ToList();

                            pendingFinishedNotifications.Add((
                                a.Id,
                                auction.FinalPrice,
                                auction.WinnerUserId,
                                usersToNotify
                            ));
                        }
                    }
                    else
                    {
                        var timeLeft = a.EndDate - now;

                        auction.Status = timeLeft.TotalHours <= 3
                            ? AuctionStatus.EndingSoon
                            : AuctionStatus.Active;
                    }

                    // تجميع Status Notification فقط
                    if (oldStatus != auction.Status &&
                        auction.Status != AuctionStatus.Finished)
                    {
                        if (participations.TryGetValue(a.Id, out var users))
                        {
                            var usersToNotify = users
                                .Append(a.SellerUserId)
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Distinct()
                                .ToList();

                            pendingStatusNotifications.Add((
                                a.Id,
                                auction.Status.ToString(),
                                usersToNotify
                            ));
                        }
                    }
                }

                // حفظ مرة واحدة فقط
                await _context.SaveChangesAsync();

                // إرسال Notifications بعد الحفظ
                foreach (var item in pendingFinishedNotifications)
                {
                    await _notification.SendAuctionFinishedNotification(
                        item.auctionId,
                        item.finalPrice,
                        item.winnerUserId,
                        item.users
                    );
                }

                foreach (var item in pendingStatusNotifications)
                {
                    await _notification.SendAuctionStatusUpdated(
                        item.auctionId,
                        item.status,
                        item.users
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAuctionStatusesAsync: {ex.Message}");
            }
        }
        private string GenerateRandomCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        public async Task<List<EndingSoonDto>> GetEndingSoonAsync(string userId, int page, int pageSize)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var data = await _auctionRepository.GetEndingSoonAsync(page, pageSize);

            return data.Select(a => new EndingSoonDto
            {
                Id = a.Id,
                Title = a.Title,
                Image = a.Image,
                CurrentPrice = a.CurrentPrice,
                EndDate = a.EndDate
            }).ToList();
        }

        public async Task<List<TrendingDto>> GetTrendingAsync(string userId, int page, int pageSize)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var data = await _auctionRepository.GetTrendingAsync(page, pageSize);

            return data.Select(a => new TrendingDto
            {
                AuctionId = a.Id,
                Title = a.Title,
                DisplayPrice =
                    a.Status == AuctionStatus.Upcoming
                        ? a.StartingPrice
                        : a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon
                            ? a.CurrentPrice
                                : 0,

                DisplayDate =
                    a.Status == AuctionStatus.Upcoming
                        ? a.StartDate
                        : a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon
                            ? a.EndDate
                                : DateTime.MinValue,
                TotalBids = a.TotalBids,
                Status = a.Status,
                Image = a.Image
            }).ToList();
        }

        public async Task<List<Dtos.UserAppDto.HomeDto.CategoryDto>> GetCategoriesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var data = await _category.GetCategoriesWithCountAsync();

            return data.Select(c => new Dtos.UserAppDto.HomeDto.CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Image = c.Image,
                AuctionCount = c.AuctionCount
            }).ToList();
        }

        public async Task<(AuthResult, List<AuctionSearchDto>?)> SearchAsync(string query , string userId , AuctionQueryDto queryDto)
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
            var auctions =
                await _auctionRepository.SearchAsync(query,
                    queryDto.CategoryId,
                    queryDto.Statuses,
                    queryDto.CityIds,
                    queryDto.MinPrice,
                    queryDto.MaxPrice,
                    queryDto.SortBy,
                    user.CityId);


            var result = auctions.Select(a => new AuctionSearchDto
            {
                AuctionId = a.Id,
                Title = a.Title,
                TotalBids = a.TotalBids,
                Status = a.Status,

                DisplayPrice =
                    a.Status == AuctionStatus.Upcoming
                        ? a.StartingPrice
                        : a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon
                            ? a.CurrentPrice
                                : 0,

                DisplayDate =
                    a.Status == AuctionStatus.Upcoming
                        ? a.StartDate
                        : a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon
                            ? a.EndDate
                                : DateTime.MinValue,

                Image = a.Image
            }).ToList();

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Search auctions retrieved successfully"
            }, result);
        }
    }
}
