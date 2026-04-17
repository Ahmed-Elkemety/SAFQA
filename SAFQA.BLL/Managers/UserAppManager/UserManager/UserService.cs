using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Dtos.UserAppDto.AccountDto;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using SAFQA.BLL.Dtos.UserAppDto.ProfileDto;
using SAFQA.BLL.Enums;
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


namespace SAFQA.BLL.Managers.UserAppManager.UserManager
{
    public class UserService : IUserService
    {
        private readonly IcategoryRepo _categoryRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IUserRepo _userRepo;

        public UserService(IcategoryRepo categoryRepo , IAuctionRepository auctionRepo, IUserRepo userRepo)
        {
            _categoryRepo = categoryRepo;
            _auctionRepo = auctionRepo;
            _userRepo = userRepo;
        }

        public async Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync()
        {
            var auctions = await _auctionRepo.GetTrendingAuctionsAsync(); // List<Auction>

            var result = auctions.Select(a => new TrendingAuctionDto
            {
                Id = a.Id,
                Title = a.Title,
                Image = a.Image
            }).ToList();

            return result;
        }

        public async Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync()
        {
            var categories = await _categoryRepo.GetCategoriesWithDetailsAsync(); // List<Category>

            var result = categories.Select(c => new CategoryWithDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Image = c.Image,
                AuctionCount = c.AuctionCount
            }).ToList();

            return result;
        }

        public PagedResult<UserListDto> GetUsers(int page, int pageSize)
        {
            var query = _userRepo.GetAll()
                .Where(u => !u.IsDeleted);

            var totalCount = query.Count();

            var users = query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(u =>
                {
                    string action;

                    if (u.Status == UserStatus.Active)
                        action = "Suspend";
                    else
                        action = "Restore";

                    return new UserListDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        Status = u.Status.ToString(),
                        Action = action
                    };
                })
                .ToList();

            return new PagedResult<UserListDto>
            {
                Data = users,
                CurrentPage = page,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasNextPage = page * pageSize < totalCount
            };
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
    }
}