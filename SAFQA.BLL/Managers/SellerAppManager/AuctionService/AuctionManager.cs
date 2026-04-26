using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.UA;
using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Enums;
using SAFQA.BLL.Help;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Category;
using SAFQA.DAL.Repository.Items;
using SAFQA.DAL.Repository.Notification;
using SAFQA.DAL.Repository.Seller;
using SAFQA.DAL.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;

namespace SAFQA.BLL.Managers.SellerAppManager.AuctionService
{
    public class AuctionManager : IAuctionManager
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IUserRepo _userRepo;
        private readonly IsellerRepo _sellerRepository;
        private readonly INotificationRepository _notification;
        private readonly SAFQA_Context _context;

        public AuctionManager(IAuctionRepository auctionRepository, IitemsRepository itemsRepository, IcategoryRepo categoryRepository, IUserRepo userRepo , IsellerRepo sellerRepository , INotificationRepository notification , SAFQA_Context context)
        {
            _auctionRepository = auctionRepository;
            _userRepo = userRepo;
            _sellerRepository = sellerRepository;
            _notification = notification;
            _context = context;
        }
        public Task<int> GetTotalSellerAuctions(int sellerId)
        {
            return _auctionRepository.CountAuctionsBySeller(sellerId);
        }

        public Task<int> GetActiveSellerAuctions(int sellerId)
        {
            return _auctionRepository.GetActiveSellerAuctions(sellerId);
        }

        public async Task<IEnumerable<TopWinnerDto>> GetWinnersBySeller(int sellerId)
        {
            var result = await _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId &&
                            a.WinnerUserId != null &&
                            !a.IsDeleted &&
                            a.Status == AuctionStatus.Finished)
                .Join(_userRepo.GetAll(),
                      a => a.WinnerUserId,
                      u => u.Id,
                      (a, u) => new { Auction = a, User = u })
                .GroupBy(x => new { x.User.Id, x.User.FullName, x.User.Email })
                .Select(g => new TopWinnerDto
                {
                    BuyerName = g.Key.FullName,
                    Email = g.Key.Email,
                    SellerCompanyName = g.Select(x => x.Auction.Seller.StoreName).FirstOrDefault(),
                    AuctionsWonCount = g.Count(),
                    TotalPaidAmount = g.Sum(x => x.Auction.FinalPrice + x.Auction.SecurityDeposit)
                })
                .OrderByDescending(x => x.TotalPaidAmount)
                .ToListAsync();

            return result.AsEnumerable(); 
        }

        public async Task<List<TopCustomerDto>> GetTopCustomers(string sellerUserId)
        {
            var users = _userRepo.GetAll().AsNoTracking();

            var query = users
                .Where(u => !u.IsDeleted)
                .Select(u => new
                {
                    User = u,
                    Auctions = u.AuctionUsers
                        .Where(au =>
                            au.Auction.Seller.UserId == sellerUserId &&
                            (au.Auction.Status == AuctionStatus.Active ||
                             au.Auction.Status == AuctionStatus.Finished))
                })
                .Select(x => new TopCustomerDto
                {
                    Name = x.User.FullName,
                    Email = x.User.Email,

                    CompanyName = x.User.Seller != null
                        ? x.User.Seller.StoreName
                        : "Customer",

                    ParticipatedAuctions = x.Auctions
                        .Select(a => a.AuctionId)
                        .Distinct()
                        .Count(),

                    TotalPaid =
                        x.Auctions.Sum(a => a.Auction.SecurityDeposit)
                        +
                        x.Auctions
                            .Where(a => a.Auction.WinnerUserId == x.User.Id)
                            .Sum(a => a.Auction.FinalPrice)
                })
                .Where(x => x.ParticipatedAuctions > 0)
                .OrderByDescending(x => x.ParticipatedAuctions);

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalAuctions()
        {
            return await _auctionRepository.GetTotalAuctionsCount();
        }
        public async Task<int> GetActiveAuctionsCount()
        {
            return await _auctionRepository.GetActiveAuctionsCount();
        }
        public async Task<int> GetExpiredAuctionsCount()
        {
            return await _auctionRepository.GetExpiredAuctionsCount();
        }
        public async Task<int> GetUpcomingAuctionsCount()
        {
            return await _auctionRepository.GetUpcomingAuctionsCount();
        }

        public async Task<List<AuctionProfitDto>> GetTopProfitableAuctions(int sellerId, int categoryId)
        {
            var validStatuses = new[] { AuctionStatus.Finished };

            var query = _auctionRepository.GetAll();

            var result = await query
                .Where(a =>
                    !a.IsDeleted &&
                    a.SellerId == sellerId &&
                    validStatuses.Contains(a.Status) &&
                    a.items.Any(i => i.Auction.CategoryId == categoryId)
                )
                .Select(a => new AuctionProfitDto
                {
                    Title = a.Title,
                    StartingPrice = a.StartingPrice,
                    FinalPrice = a.FinalPrice,
                    Profit = a.FinalPrice - a.StartingPrice,
                    WinnerName = a.AuctionUsers
                        .Where(au => au.UserId == a.WinnerUserId)
                        .Select(au => au.User.FullName)
                        .FirstOrDefault()
                })
                .OrderByDescending(a => a.FinalPrice - a.StartingPrice)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CategoryPercentageDto>> GetCategoryPercentageBySeller(int sellerId)
        {
            var auctions = _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId
                            && !a.IsDeleted
                            && (a.Status == AuctionStatus.Active || a.Status == AuctionStatus.Finished))
                .Include(a => a.items)
                .ThenInclude(i => i.Auction.Category);

            var allItems = auctions.SelectMany(a => a.items)
                                   .Where(i => i.Auction.CategoryId != null);

            var totalItems = allItems.Count();

            
            if (totalItems == 0)
                return new List<CategoryPercentageDto>();

            var result = allItems
                .GroupBy(i => i.Auction.Category.Name)
                .Select(g => new CategoryPercentageDto
                {
                    CategoryName = g.Key,
                    Percentage = Math.Round((double)g.Count() / totalItems * 100, 2) 
                })
                .OrderByDescending(c => c.Percentage)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<AuctionBidsDto>> GetSellerAuctionsBids(int sellerId)
        {
            var query = _auctionRepository.GetAll();

            var result = await query
                .Where(a =>
                    !a.IsDeleted &&
                    a.SellerId == sellerId &&
                    a.Bids.Any() 
                )
                .Select(a => new AuctionBidsDto
                {
                    Title = a.Title,
                    StartDate = a.StartDate,
                    TotalBids = a.Bids.Count()
                })
                .OrderByDescending(a => a.TotalBids)
                .ToListAsync();

            return result;
        }

        public IEnumerable<MonthlyEarningDto> GetMonthlyEarningsByCategory(int sellerId, int categoryId)
        {
            var auctions = _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId
                && a.items.Any(i => i.Auction.CategoryId == categoryId)
                && !a.IsDeleted)
                                .ToList();

            var grouped = auctions
                .GroupBy(a => new { a.EndDate.Year, a.EndDate.Month })
                .Select(g => new MonthlyEarningDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalEarnings = g.Sum(a => a.FinalPrice), // أو استخدم Transactions لو عايز دقة أكبر
                    AuctionCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return grouped;
        }

        public IEnumerable<PopularProductsDto> GetMostPopularProductsBySeller(int sellerId, int topCount = 5)
        {
            var sellerAuctions = _auctionRepository.GetAll()
                .Where(a => a.SellerId == sellerId);


            var productViews = sellerAuctions
                .SelectMany(a => a.items, (auction, item) => new PopularProductsDto
                {
                    Title = item.title,
                    Description = item.Description,
                    ViewCount = auction.ViewsCount 
                })
                .OrderByDescending(p => p.ViewCount) 
                .Take(topCount)
                .ToList();

            return productViews;
        }

        public async Task<PagedResult<SellerActionHistoryDto>> GetHistory(
            string userId,
            AuctionStatus? status,
            int page,
            int pageSize)
        {
            var query = _auctionRepository.GetSellerAuctions(userId);

            var mappedQuery = query.Select(a => new SellerActionHistoryDto
            {
                AuctionId = a.Id,
                Title = a.Title,

                DisplayPrice =
                a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled ? a.StartingPrice :
                a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon ? a.CurrentPrice :
                a.Status == AuctionStatus.Finished ? a.FinalPrice :
                0,

                DisplayDate =
                a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled ? a.StartDate :
                a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon ? a.EndDate :
                a.Status == AuctionStatus.Finished ? a.EndDate :
                a.StartDate,

                TotalBids = a.TotalBids,
                Status = a.Status,

                Image = a.Image
            });

            if (status.HasValue)
            {
                mappedQuery = mappedQuery
                    .Where(x => x.Status == status.Value);
            }

            var totalCount = await mappedQuery.CountAsync();

            var data = await mappedQuery
                .OrderByDescending(x => x.DisplayDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<SellerActionHistoryDto>
            {
                Data = data,
                CurrentPage = page,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasNextPage = page * pageSize < totalCount
            };
        }

        public PagedResult<ActiveAuctionDto> GetActiveAuctions(int page, int pageSize)
        {
            var query = _auctionRepository.GetAll(); 

            var filteredQuery = query
                .Where(a =>
                    a.Status == AuctionStatus.Active &&
                    !a.IsDeleted &&
                    a.EndDate > DateTime.UtcNow
                );

            var totalCount = filteredQuery.Count();

            var data = filteredQuery
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ActiveAuctionDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    CurrentPrice = a.CurrentPrice,
                    EndDate = a.EndDate,
                    ImageBase64 = a.Image != null
                        ? Convert.ToBase64String(a.Image)
                        : null
                })
                .ToList();

            return new PagedResult<ActiveAuctionDto>
            {
                Data = data,
                TotalCount = totalCount,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasNextPage = page * pageSize < totalCount
            };
        }

        public void ForceExpireAuction(int auctionId)
        {
            var auction = _auctionRepository.GetById(auctionId);

            if (auction == null)
                throw new Exception("Auction not found");

            if (auction.Status != AuctionStatus.Active)
                throw new Exception("Auction is not active");

            if (auction.IsDeleted)
                throw new Exception("Auction already deleted");

            auction.Status = AuctionStatus.Finished;
            auction.EndDate = DateTime.UtcNow;
            auction.UpdatedAt = DateTime.UtcNow;

            _auctionRepository.Update(auction);
        }

        public PagedResult<ExpiredAuctionsDto> GetExpiredAuctions(int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _auctionRepository.GetAll()
                .Where(a => !a.IsDeleted
                            && a.EndDate < DateTime.Now
                            && a.Status == AuctionStatus.Finished)
                .Select(a => new ExpiredAuctionsDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Image = a.Image,
                    EndDate = a.EndDate,
                    Price = a.FinalPrice > 0 ? a.FinalPrice : a.CurrentPrice
                });

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<ExpiredAuctionsDto>
            {
                Data = data,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                HasNextPage = page < totalPages
            };
        }

        public void DeleteAuctionPermanently(int id)
        {
            var auction = _auctionRepository.GetById(id);

            if (auction == null)
                throw new Exception("Auction not found");

            auction.IsDeleted = true;
            auction.DeletedAt = DateTime.Now.ToString();
            auction.UpdatedAt = DateTime.Now;

            _auctionRepository.Update(auction);
        }

        public PagedResult<RejectedDeletedAuctionDto> GetRejectedDeletedAuctions(int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _auctionRepository.GetAll()
                .Where(a =>
                    a.IsDeleted == true
                    || a.Status == AuctionStatus.Cancelled
                );

            var totalCount = query.Count();

            var data = query
                .OrderByDescending(a => a.EndDate) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new RejectedDeletedAuctionDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Image = a.Image,
                    CurrentPrice = a.CurrentPrice,
                    EndDate = a.EndDate
                })
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedResult<RejectedDeletedAuctionDto>
            {
                Data = data,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                HasNextPage = page < totalPages
            };
        }
        public void DeleteAuctionPermanentlyy(int id)
        {
            var auction = _auctionRepository.GetById(id);

            if (auction == null)
                throw new Exception("Auction not found");

            _auctionRepository.Delete(auction);
        }

        public async Task<AuthResult> CreateAuction(CreateAuctionDto dto , string UserId)
        {
            if (UserId == null)
                return new AuthResult { IsSuccess = false, Message = "Unauthorized" };

            // 2️⃣ Get Seller
            var seller = await _sellerRepository.GetByUserIdAsync(UserId);

            if (seller == null)
                return new AuthResult { IsSuccess = false, Message = "Seller not found" };

            var errors = ValidateAuction(dto);
            if (errors.Any())
                return new AuthResult { Errors = errors };



            var auction = new Auction
            {
                Title = dto.Title,
                Description = dto.Description,
                StartingPrice = dto.StartingPrice,
                CurrentPrice = dto.StartingPrice,
                FinalPrice = dto.StartingPrice,
                BidIncrement = dto.BidIncrement,
                SecurityDeposit = dto.StartingPrice * 0.1m,
                CategoryId = dto.CategoryId,

                StartDate = dto.StartDate,
                EndDate = dto.EndDate,

                CreatedAt = DateTime.UtcNow.AddHours(2),
                SellerId = seller.Id,
                items = new List<Item>()
            };


            // 4️⃣ Head Image Validation
            var headVal = FileValidator.Validate(dto.Image);
            if (!headVal.ok)
                return new AuthResult { IsSuccess = false, Message = headVal.error };

            using (var ms = new MemoryStream())
            {
                await dto.Image.CopyToAsync(ms);
                auction.Image = ms.ToArray();
            }

            // 5️⃣ Items
            foreach (var itemDto in dto.Items)
            {
                var item = new Item
                {
                    title = itemDto.Title,
                    Count = itemDto.Count,
                    Description = itemDto.Description,
                    WarrantyInfo = itemDto.WarrantyInfo,
                    Condition = itemDto.Condition,
                    images = new List<Images>(),
                    itemAttributesValues = new List<ItemAttributesValue>()
                };

                // Images validation
                foreach (var img in itemDto.Images)
                {
                    var imgVal = FileValidator.Validate(img);
                    if (!imgVal.ok)

                        return new AuthResult { IsSuccess = false, Message = imgVal.error };

                    using var ms = new MemoryStream();
                    await img.CopyToAsync(ms);

                    item.images.Add(new Images
                    {
                        Image = ms.ToArray(),
                        item = item
                    });
                }

                // Attributes
                foreach (var attr in itemDto.Attributes)
                {
                    item.itemAttributesValues.Add(new ItemAttributesValue
                    {
                        CategoryAttributeId = attr.CategoryAttributeId,
                        value = attr.Value,
                        Item = item
                    });
                }

                auction.items.Add(item);
            }

            // 6️⃣ Save
            await _auctionRepository.AddAsync(auction);


            var notification = new DAL.Models.Notification
            {
                Title = "Auction Created",
                Message = $"Your auction '{auction.Title}' has been created",
                UserId = UserId,
                notificationType = NotificationTypes.AuctionsStatus,
                ReferenceId = auction.Id,
                IsRead = false,
                CreatedAt = DateTime.Now.AddHours(2)
            };

            await _notification.AddAsync(notification);
            await _notification.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Auction created successfully"
            };
        }

        public async Task<AuthResult> EditAuction(int auctionId, EditAuctionDto dto, string userId)
        {
            var auction = await _auctionRepository.GetByIdWithDetailsAsync(auctionId);

            if (auction == null)
                return new AuthResult { IsSuccess = false, Message = "Auction not found" };

            if (auction.Status == AuctionStatus.Active)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Cannot edit active auction" }
                };

            // ✅ Authorization (safe)
            if (auction.Seller == null || auction.Seller.UserId != userId)
                return new AuthResult { IsSuccess = false, Message = "Unauthorized" };

            var errors = ValidateAuction(dto);
            if (errors.Any())
                return new AuthResult { IsSuccess = false, Errors = errors };

            // ✅ Update Auction
            auction.Title = dto.Title;
            auction.Description = dto.Description;
            auction.UpdatedAt = DateTime.UtcNow.AddHours(2);
            auction.CategoryId = dto.CategoryId;

            // ✅ Head Image
            if (dto.Image != null)
            {
                var val = FileValidator.Validate(dto.Image);
                if (!val.ok)
                    return new AuthResult { IsSuccess = false, Message = val.error };

                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                auction.Image = ms.ToArray();
            }

            // =========================================
            // 🟢 1. existing IDs (صح 100%)
            // =========================================
            var existingIds = dto.Items?
                .Where(x => x.id.HasValue)
                .Select(x => x.id.Value)
                .ToList() ?? new List<int>();

            // =========================================
            // 🔴 2. Delete removed items (safe)
            // =========================================
            var toRemove = auction.items
                .Where(i => !existingIds.Contains(i.Id))
                .ToList();

            foreach (var item in toRemove)
            {
                _context.images.RemoveRange(item.images);
                _context.itemAttributesValues.RemoveRange(item.itemAttributesValues);
                _context.Items.Remove(item);
            }

            // =========================================
            // 🟡 3. Add / Update
            // =========================================
            foreach (var itemDto in dto.Items ?? new List<EditItemDto>())
            {
                Item item;

                if (itemDto.id.HasValue)
                {
                    // ✅ بدل First (عشان متكراشّش)
                    item = auction.items
                        .FirstOrDefault(x => x.Id == itemDto.id.Value);

                    if (item == null)
                    {
                        return new AuthResult
                        {
                            IsSuccess = false,
                            Message = $"Item with id {itemDto.id} not found in this auction"
                        };
                    }

                    // 🟢 Update
                    item.title = itemDto.Title;
                    item.Description = itemDto.Description;
                    item.Count = itemDto.Count;
                    item.WarrantyInfo = itemDto.WarrantyInfo;
                    item.Condition = itemDto.Condition;

                    // Reset attributes
                    _context.itemAttributesValues.RemoveRange(item.itemAttributesValues);
                    item.itemAttributesValues.Clear();
                }
                else
                {
                    // 🆕 Add
                    item = new Item
                    {
                        title = itemDto.Title,
                        Description = itemDto.Description,
                        Count = itemDto.Count,
                        WarrantyInfo = itemDto.WarrantyInfo,
                        Condition = itemDto.Condition,
                        images = new List<Images>(),
                        itemAttributesValues = new List<ItemAttributesValue>()
                    };

                    auction.items.Add(item);
                }

                // =========================================
                // 🖼️ Images (safe)
                // =========================================
                if (itemDto.Images != null)
                {
                    _context.images.RemoveRange(item.images);
                    item.images.Clear();

                    foreach (var img in itemDto.Images)
                    {
                        var val = FileValidator.Validate(img);
                        if (!val.ok)
                            return new AuthResult { IsSuccess = false, Message = val.error };

                        using var ms = new MemoryStream();
                        await img.CopyToAsync(ms);

                        item.images.Add(new Images
                        {
                            Image = ms.ToArray(),
                            item = item
                        });
                    }
                }

                // =========================================
                // 🧩 Attributes
                // =========================================
                if (itemDto.Attributes != null)
                {
                    foreach (var attr in itemDto.Attributes)
                    {
                        item.itemAttributesValues.Add(new ItemAttributesValue
                        {
                            CategoryAttributeId = attr.CategoryAttributeId,
                            value = attr.Value,
                            Item = item
                        });
                    }
                }
            }

            await _auctionRepository.SaveChangesAsync();

            // =========================================
            // 🔔 Notification
            // =========================================
            var notification = new DAL.Models.Notification
            {
                Title = "Auction Edited",
                Message = $"Your auction '{auction.Title}' has been edited",
                UserId = userId,
                notificationType = NotificationTypes.AuctionsStatus,
                ReferenceId = auction.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notification.AddAsync(notification);
            await _notification.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Auction updated successfully"
            };
        }

        public async Task<ViewAuctionDto> GetAuction(int id,string userid)
        {
            if (userid == null)
                throw new Exception ( "Unauthorized") ;

            var auction = await _auctionRepository.GetWithDetailsAsync(id);

            if (auction == null)
                return null;

            var result = new ViewAuctionDto
            {
                Title = auction.Title,
                Description = auction.Description,
                Image = auction.Image,
                BidIncrement = auction.BidIncrement,
                StartingPrice = auction.StartingPrice,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate,
                CategoryId = auction.CategoryId,

                Items = auction.items.Select(i => new ViewItemDto
                {
                    id = i.Id,
                    Title = i.title,
                    Description = i.Description,
                    Count = i.Count,
                    WarrantyInfo = i.WarrantyInfo,
                    Condition = i.Condition,

                    Images = i.images?.Select(img => img.Image).ToList()
                             ?? new List<byte[]>(),

                    Attributes = i.itemAttributesValues?.Select(attr => new ItemAttributeDto
                    {
                        CategoryAttributeId = attr.CategoryAttributeId,
                        Image = attr.categoryAttributes.Image,
                        Value = attr.value
                    }).ToList()
                    ?? new List<ItemAttributeDto>()

                }).ToList()
            };
            return result;
        }

        public async Task<AuthResult> DeleteAuction(int id, string userId)
        {
            var auction = await _auctionRepository.GetByIdAsync(id);
            var seller = await _sellerRepository.GetByUserIdAsync(userId);


            if (auction == null)
                return new AuthResult { Errors = { "Not Found" } };

            if (auction.SellerId != seller.Id)
                return new AuthResult { Errors = { "Unauthorized" } };

            if (auction.Status == AuctionStatus.Active)
                return new AuthResult { Errors = { "Cannot delete active auction" } };


            auction.IsDeleted = true;
            auction.DeletedAt = DateTime.UtcNow.AddHours(2).ToString();

            _auctionRepository.Update(auction);
            await _auctionRepository.SaveChangesAsync();

            var notification = new DAL.Models.Notification
            {
                Title = "Auction Created",
                Message = $"Your auction '{auction.Title}' has been Deleted",
                UserId = userId,
                notificationType = NotificationTypes.AuctionsStatus,
                ReferenceId = auction.Id,
                IsRead = false,
                CreatedAt = DateTime.Now.AddHours(2)
            };

            await _notification.AddAsync(notification);
            await _notification.SaveChangesAsync();

            return new AuthResult { IsSuccess = true, Message = "Deleted Successfully" };
        }




        private List<string> ValidateAuction(CreateAuctionDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required");

            if (dto.StartingPrice <= 0)
                errors.Add("Starting price must be greater than 0");

            if (dto.BidIncrement <= 0)
                errors.Add("Bid increment must be greater than 0");

            if (dto.EndDate <= dto.StartDate)
                errors.Add("End date must be after start date");

            if (dto.Items == null || !dto.Items.Any())
                errors.Add("At least one item is required");

            return errors;
        }

        private List<string> ValidateAuction(EditAuctionDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required");

            if (dto.Items == null || !dto.Items.Any())
                errors.Add("At least one item is required");

            return errors;
        }
    }
}