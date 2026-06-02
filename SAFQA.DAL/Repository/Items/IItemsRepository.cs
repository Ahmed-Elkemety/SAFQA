using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Items
{
    public interface IitemsRepository
    {
        IQueryable<Item> GetAll();
        Item GetById(int Id);
        IQueryable<Item> GetSellerProducts(string userId);
        IQueryable<Item> GetProductsByCategory(string sellerId, string categoryName);
        IQueryable<Item> GetMostPopularProducts(string sellerId, int top = 5);
    }
}