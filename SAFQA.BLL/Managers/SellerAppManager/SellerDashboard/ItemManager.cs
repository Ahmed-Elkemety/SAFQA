using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard
{
    public class ItemManager : IItemManager
    {
        private readonly IitemsRepository _itemsRepository;

        public ItemManager(IitemsRepository itemsRepository)
        {
            _itemsRepository = itemsRepository;
        }

        public IQueryable<Item> GetSellerProducts(int sellerId)
        {
            return _itemsRepository.GetSellerProducts(sellerId);
        }

        public IQueryable<Item> GetProductsByCategory(int sellerId, string categoryName)
        {
            return _itemsRepository.GetProductsByCategory(sellerId, categoryName);
        }

        public IQueryable<Item> GetMostPopularProducts(int sellerId, int top = 5)
        {
            return _itemsRepository.GetMostPopularProducts(sellerId, top);
        }
    }
}
