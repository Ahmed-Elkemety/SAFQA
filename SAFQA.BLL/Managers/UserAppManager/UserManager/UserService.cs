using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Dtos.UserAppDto.AccountDto;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using SAFQA.BLL.Dtos.UserAppDto.ProfileDto;
using SAFQA.DAL.Enums;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Repository;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Category;
using SAFQA.DAL.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;
using SAFQA.DAL.Models;
using SAFQA.DAL.Database;


namespace SAFQA.BLL.Managers.UserAppManager.UserManager
{
    public class UserService : IUserService
    {
        private readonly IcategoryRepo _categoryRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IUserRepo _userRepo;
        private readonly SAFQA_Context _context;

        public UserService(IcategoryRepo categoryRepo , IAuctionRepository auctionRepo, IUserRepo userRepo ,SAFQA_Context Context)
        {
            _categoryRepo = categoryRepo;
            _auctionRepo = auctionRepo;
            _userRepo = userRepo;
            _context = Context;
        }

        public bool ChangeStatus(string userId)
        {
            var user = _userRepo.GetById(userId);

            if (user == null)
                return false;

            user.Status = user.Status switch
            {
                UserStatus.Active => UserStatus.Blocked,
                UserStatus.Blocked => UserStatus.Active,
                UserStatus.Inactive => UserStatus.Active,
                _ => user.Status
            };

            user.UpdatedAt = DateTime.UtcNow;

            _userRepo.Update(user);

            return true;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepo.GetTotalUsers();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _userRepo.GetActiveUsersCount();
        }

        public async Task<int> GetBlockedUsersCountAsync()
        {
            return await _userRepo.GetBlockedUsersCount();
        }

        // 🟢 Profile
        public async Task<(AuthResult, UserProfileDto?)> GetProfile(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                }, null);
            }

            var data = new UserProfileDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Image = user.Image
            };

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Profile retrieved successfully"
            }, data);
        }

        // 🟢 Account Details
        public async Task<(AuthResult, UserAccountDto?)> GetAccount(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                }, null);
            }

            var data = new UserAccountDto
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender.ToString(),
                BirthDate = user.BirthDate,
                City = user.City?.Name,
                Image = user.Image
            };

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Account retrieved successfully"
            }, data);
        }

        // 🟢 Edit Account
        public async Task<AuthResult> EditAccount(string userId, EditAccountDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add("Full name is required");

            if (errors.Any())
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Validation failed",
                    Errors = errors
                };

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = dto.Gender;
            user.BirthDate = dto.BirthDate;
            user.CityId = dto.CityId;

            if (dto.Image != null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                user.Image = ms.ToArray();
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Account updated successfully"
            };
        }
        public async Task<AuthResult> FollowSeller(string userId, int sellerId)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s =>
                    s.Id == sellerId &&
                    !s.IsDeleted);

            if (seller == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Seller not found"
                };
            }

            var followExists = await _context.UserFollowers
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.SellerId == sellerId);

            if (followExists)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "You already follow this seller"
                };
            }

            await _context.UserFollowers.AddAsync(new UserFollowers
            {
                UserId = userId,
                SellerId = sellerId,
                FollowedAt = DateTime.UtcNow
            });

            seller.Followers++;

            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Seller followed successfully"
            };
        }

        public async Task<AuthResult> UnfollowSeller(string userId, int sellerId)
        {
            var follow = await _context.UserFollowers
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SellerId == sellerId);

            if (follow == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "You are not following this seller"
                };
            }

            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.Id == sellerId);

            _context.UserFollowers.Remove(follow);

            if (seller != null && seller.Followers > 0)
                seller.Followers--;

            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Seller unfollowed successfully"
            };
        }
        public async Task<AuthResult> RemoveFavorite(string userId, int auctionId)
        {
            var favorite = await _context.auctionLikes
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.AuctionId == auctionId);

            if (favorite == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Auction is not in favorites"
                };
            }

            var auction = await _context.Auctions
                .FirstOrDefaultAsync(x => x.Id == auctionId);

            _context.auctionLikes.Remove(favorite);

            if (auction != null && auction.LikesCount > 0)
                auction.LikesCount--;

            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Removed from favorites successfully"
            };
        }
    }
}