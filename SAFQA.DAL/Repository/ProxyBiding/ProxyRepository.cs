using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.ProxyBiding
{
    public class ProxyRepository : IProxyRepository
    {
        private readonly SAFQA_Context _context;

        public ProxyRepository(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task<ProxyBidding?> GetAsync(int auctionId, string userId)
        {
            return await _context.proxyBiddings
                .FirstOrDefaultAsync(p => p.AuctionId == auctionId && p.UserId == userId);
        }

        public async Task AddAsync(ProxyBidding proxy)
        {
            await _context.proxyBiddings.AddAsync(proxy);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
