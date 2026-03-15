using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.ItemManager.ItemManager
{
    public interface IItemManager
    {
        IQueryable<Item> GetSellerProducts(int sellerId);

        IQueryable<Item> GetProductsByCategory(int sellerId, string categoryName);

        IQueryable<Item> GetMostPopularProducts(int sellerId, int top = 5);
        Task<List<CategoryStatsDto>> GetSellerCategoryStats(int sellerId);
    }
}
