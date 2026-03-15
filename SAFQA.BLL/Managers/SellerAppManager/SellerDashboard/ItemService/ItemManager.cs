using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard.ItemRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.ItemService.ItemManager.ItemManager
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

        public async Task<List<CategoryStatsDto>> GetSellerCategoryStats(int sellerId)
        {
            var data = await _itemsRepository.GetSellerCategoryProductCounts(sellerId);

            var total = data.Sum(x => x.Count);

            return data.Select(x => new CategoryStatsDto
            {
                CategoryName = x.CategoryName,
                ProductCount = x.Count,
                Percentage = total == 0 ? 0 : (double)x.Count / total * 100
            }).OrderByDescending(x => x.Percentage)
              .ToList();
        }
    }
}