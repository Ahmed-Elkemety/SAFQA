using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.ItemService
{
    public class ItemManager : IItemManager
    {
        private readonly IitemsRepository _itemsRepository;

        public ItemManager(IitemsRepository itemsRepository)
        {
            _itemsRepository = itemsRepository;
        }

        public IQueryable<Item> GetSellerProducts(string userId)
        {
            return _itemsRepository.GetSellerProducts(userId);
        }

        public IQueryable<Item> GetProductsByCategory(string userId, string categoryName)
        {
            return _itemsRepository.GetProductsByCategory(userId, categoryName);
        }

        public IQueryable<Item> GetMostPopularProducts(string userId, int top = 5)
        {
            return _itemsRepository.GetMostPopularProducts(userId, top);
        }

    }
}