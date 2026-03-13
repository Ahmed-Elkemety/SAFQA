using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard;

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
    }
}
