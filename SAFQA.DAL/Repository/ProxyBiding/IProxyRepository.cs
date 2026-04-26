using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.ProxyBiding
{
    public interface IProxyRepository
    {
        Task<ProxyBidding?> GetAsync(int auctionId, string userId);
        Task SaveChangesAsync();
        Task AddAsync(ProxyBidding proxy);
    }
}
