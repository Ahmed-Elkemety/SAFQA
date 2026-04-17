using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.UserAppDto.AuctionDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;

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
        public async Task<(AuthResult, object)> GetAuctionsByCategory(int categoryId, string userId, int pageNumber, int pageSize)
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

            var (auctions, totalCount) = await _auctionRepository
                .GetAuctionsByCategoryId(categoryId, pageNumber, pageSize);

            if (!auctions.Any())
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "No auctions found for this category"
                }, null);

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Auctions retrieved successfully",
            },
            new
            {
                Data = auctions,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
    }
}
