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
        IQueryable<Item> GetSellerProducts(int sellerId);
        IQueryable<Item> GetProductsByCategory(int sellerId, string categoryName);
        IQueryable<Item> GetMostPopularProducts(int sellerId, int top = 5);
        Task<List<(string CategoryName, int Count)>> GetSellerCategoryProductCounts(int sellerId);
        Task<List<(string CategoryName, int Count)>> GetCategoryCountsBySellerAsync(int sellerId);
    }
}
