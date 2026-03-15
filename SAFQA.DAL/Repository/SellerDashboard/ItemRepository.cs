using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard
{
    public class ItemRepository : IitemsRepository
    {   
        private readonly SAFQA_Context _context;
        public ItemRepository(SAFQA_Context context) 
        {
            _context = context;
        }
        public IQueryable<Item> GetMostPopularProducts(int sellerId, int top = 5)
        {
            return _context.Items
                            .Where(i => i.Auction.SellerId == sellerId)
                            .OrderByDescending(i => i.Auction.TotalBids)
                            .Take(top);
        }

        public IQueryable<Item> GetProductsByCategory(int sellerId, string categoryName)
        {
            return _context.Items
                .Where(i => i.Auction.SellerId == sellerId && i.Category.Name == categoryName);
        }
        
        //  return _context.Items.Where(a => a.CategoryId == categoryId);  
        public IQueryable<Item> GetSellerProducts(int sellerId)
        {
            return _context.Items
                    .Where(a => a.Auction.SellerId == sellerId);
        }
    }
}
