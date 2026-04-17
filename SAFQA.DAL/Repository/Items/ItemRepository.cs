using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Items
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
                .Where(i => i.Auction.SellerId == sellerId && i.Auction.Category.Name == categoryName);
        }
        
        public IQueryable<Item> GetSellerProducts(int sellerId)
        {
            return _context.Items
                    .Where(a => a.Auction.SellerId == sellerId);
        }

        public IQueryable<Item> GetAll()
        {
            return _context.Items;
        }

        public Item GetById(int Id)
        {
            return _context.Items.FirstOrDefault(a => a.Id == Id);
        }
    }
}
