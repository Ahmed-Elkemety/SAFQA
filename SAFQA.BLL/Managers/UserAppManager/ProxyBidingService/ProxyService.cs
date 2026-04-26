using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.UserAppDto.ProxyBidding;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.ProxyBiding;

namespace SAFQA.BLL.Managers.UserAppManager.ProxyBidingService
{
    public class ProxyService : IProxyService
    {  
        private readonly IProxyRepository _repo;
        private readonly SAFQA_Context _context;

        public ProxyService(IProxyRepository repo,SAFQA_Context context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<AuthResult> ActivateAsync(int auctionId, string userId)
        {
            var proxy = await _repo.GetAsync(auctionId, userId);

            if (proxy == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Proxy not found"
                };
            }

            proxy.Status = ProxyStatus.Active;
            proxy.UpdateAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Proxy activated"
            };
        }

        public async Task<AuthResult> DeactivateAsync(int auctionId, string userId)
        {
            var proxy = await _repo.GetAsync(auctionId, userId);

            if (proxy == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Proxy not found"
                };
            }

            proxy.Status = ProxyStatus.Inactive;
            proxy.UpdateAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Proxy deactivated"
            };
        }

        public async Task<AuthResult> CreateAsync(CreateProxyDto dto, string userId)
        {
            var existing = await _repo.GetAsync(dto.AuctionId, userId);

            if (existing != null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Proxy already exists"
                };
            }

            // validation
            if (dto.Step <= 0 || dto.Max <= 0)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid values"
                };
            }

            var auction = await _context.Auctions.FindAsync(dto.AuctionId);

            if (auction == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Auction not found"
                };
            }

            if (dto.Max <= auction.CurrentPrice)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Max must be greater than current price"
                };
            }

            var proxy = new ProxyBidding
            {
                AuctionId = dto.AuctionId,
                UserId = userId,
                Max = dto.Max,
                Step = dto.Step,
                Status = ProxyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            await _repo.AddAsync(proxy);
            await _repo.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Proxy created successfully"
            };
        }

        public async Task<AuthResult> UpdateAsync(UpdateProxyDto dto, string userId)
        {
            var proxy = await _repo.GetAsync(dto.AuctionId, userId);

            if (proxy == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Proxy not found"
                };
            }

            if (dto.Step <= 0 || dto.Max <= 0)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid values"
                };
            }

            var auction = await _context.Auctions.FindAsync(dto.AuctionId);

            if (auction == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Auction not found"
                };
            }

            if (dto.Max <= auction.CurrentPrice)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Max must be greater than current price"
                };
            }

            proxy.Max = dto.Max;
            proxy.Step = dto.Step;
            proxy.UpdateAt = DateTime.UtcNow;
            proxy.Status = ProxyStatus.Active;

            await _repo.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Proxy updated successfully"
            };
        }
    }
}
