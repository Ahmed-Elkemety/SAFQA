using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.ItemService
{
    public interface IItemManager
    {
        IQueryable<Item> GetSellerProducts(string userId);

        IQueryable<Item> GetProductsByCategory(string SellerId, string categoryName);

        IQueryable<Item> GetMostPopularProducts(string userId, int top = 5);
    }
}