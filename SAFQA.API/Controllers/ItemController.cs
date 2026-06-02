using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.SellerAppManager.ItemService;
using System.Security.Claims;

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
        public IActionResult GetSellerProducts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var products = _itemManager.GetSellerProducts(userId).ToList();

            return Ok(products);
        }

        [HttpGet("products-by-category")]
        public IActionResult GetProductsByCategory(string categoryName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var products = _itemManager
                .GetProductsByCategory(userId, categoryName)
                .ToList();

            return Ok(products);
        }

        [HttpGet("most-popular-products")]
        public IActionResult GetMostPopularProducts(int top = 5)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var products = _itemManager
                .GetMostPopularProducts(userId, top)
                .ToList();

            return Ok(products);
        }
    }
}