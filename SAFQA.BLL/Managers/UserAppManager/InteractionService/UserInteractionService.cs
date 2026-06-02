using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.UserAppManager.InteractionService
{
    // ===============================
    // SERVICE IMPLEMENTATION (BLL)
    // ===============================

    public class UserInteractionService : IUserInteractionService
    {
        private readonly SAFQA_Context _context;

        public UserInteractionService(SAFQA_Context context)
        {
            _context = context;
        }

        // ===============================
        // ADD FAVORITE
        // ===============================
        public async Task<AuthResult> AddFavoriteAsync(int auctionId, string userId)
        {
            var result = new AuthResult();

            var auction = await _context.Auctions
                .FirstOrDefaultAsync(a => a.Id == auctionId && !a.IsDeleted);

            if (auction == null)
            {
                result.Errors.Add("Auction not found");
                return result;
            }

            var alreadyExists = await _context.Set<AuctionLike>()
                .AnyAsync(x => x.AuctionId == auctionId && x.UserId == userId);

            if (alreadyExists)
            {
                result.Errors.Add("Auction already in favorites");
                return result;
            }

            var favorite = new AuctionLike
            {
                AuctionId = auctionId,
                UserId = userId
            };

            await _context.Set<AuctionLike>().AddAsync(favorite);

            auction.LikesCount++;

            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            return result;
        }

        // ===============================
        // ADD VIEW
        // ===============================
        public async Task<AuthResult> AddViewAsync(int auctionId, string userId, string deviceType)
        {
            var result = new AuthResult();

            var auction = await _context.Auctions
                .FirstOrDefaultAsync(a => a.Id == auctionId && !a.IsDeleted);

            if (auction == null)
            {
                result.Errors.Add("Auction not found");
                return result;
            }

            var view = new AuctionView
            {
                AuctionId = auctionId,
                UserId = userId,
                DeviceType = deviceType,
                ViewedAt = DateTime.UtcNow
            };

            await _context.Set<AuctionView>().AddAsync(view);

            auction.ViewsCount++;

            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            return result;
        }
    }
}
