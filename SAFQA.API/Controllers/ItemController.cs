using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.SellerAppManager.ItemService;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemManager _itemManager;

        public ItemController(IItemManager itemManager)
        {
            _itemManager = itemManager;
        }

        [HttpGet("seller-products/{sellerId}")]
        public IActionResult GetSellerProducts(int sellerId)
        {
            var products = _itemManager.GetSellerProducts(sellerId).ToList();

            return Ok(products);
        }

        [HttpGet("products-by-category")]
        public IActionResult GetProductsByCategory(int sellerId, string categoryName)
        {
            var products = _itemManager
                .GetProductsByCategory(sellerId, categoryName)
                .ToList();

            return Ok(products);
        }

        [HttpGet("most-popular-products/{sellerId}")]
        public IActionResult GetMostPopularProducts(int sellerId, int top = 5)
        {
            var products = _itemManager
                .GetMostPopularProducts(sellerId, top)
                .ToList();

            return Ok(products);
        }

        [HttpGet("seller-categories-stats/{sellerId}")]
        public async Task<IActionResult> GetSellerCategoryStats(int sellerId)
        {
            var result = await _itemManager.GetSellerCategoryStats(sellerId);

            return Ok(result);
        }

        [HttpGet("category-percentage/{sellerId}")]
        public async Task<IActionResult> GetCategoryPercentage(int sellerId)
        {
            var result = await _itemManager.GetCategoryPercentageAsync(sellerId);

            if (result == null || !result.Any())
                return Ok(new List<CategoryPercentageDto>()); // أو ممكن ترجع NoContent()

            return Ok(result);
        }
    }
}
