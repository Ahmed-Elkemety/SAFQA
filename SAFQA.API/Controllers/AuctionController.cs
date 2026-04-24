using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Dtos.UserAppDto.AuctionDto;
using SAFQA.DAL.Enums;
using SAFQA.BLL.Managers.SellerAppManager.AuctionService;
using SAFQA.BLL.Managers.SellerAppManager.CategoryService;
using SAFQA.BLL.Managers.UserAppManager.AuctionManager;
using SAFQA.DAL.Repository.Category;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionManager _auctionManager;
        private readonly IAuctionManagerU _auctionManagerU;
        private readonly ICategoryService _category;

        public AuctionController(IAuctionManager auctionManager,IAuctionManagerU auctionManagerU, ICategoryService category)
        {
            _auctionManager = auctionManager;
            _auctionManagerU = auctionManagerU;
            _category = category;
        }

        // GET: api/Auction/active/5
        [HttpGet("Total_active/{sellerId}")]
        public async Task<IActionResult> GetActiveAuctions(int sellerId)
        {
            var activeAuctions = await _auctionManager.GetActiveSellerAuctions(sellerId);
            return Ok(activeAuctions);
        }

        // GET: api/Auction/total/5
        [HttpGet("total/{sellerId}")]
        public async Task<IActionResult> GetTotalAuctions(int sellerId)
        {
            var total = await _auctionManager.GetTotalSellerAuctions(sellerId);
            return Ok(total);
        }


        [HttpGet("{sellerUserId}/Top customers")]
        public async Task<IActionResult> GetSellerCustomers(string sellerUserId)
        {
            var result = await _auctionManager.GetTopCustomers(sellerUserId);

            return Ok(result);
        }

        [HttpGet("total-auctions")]
        public async Task<IActionResult> GetTotalAuctions()
        {
            int total = await _auctionManager.GetTotalAuctions();
            return Ok(new { totalAuctions = total });
        }

        [HttpGet("active-auctions")]
        public async Task<IActionResult> GetActiveAuctionsCount()
        {
            int count = await _auctionManager.GetActiveAuctionsCount();
            return Ok(new { activeAuctions = count });
        }

        [HttpGet("expired-auctions")]
        public async Task<IActionResult> GetExpiredAuctionsCount()
        {
            int count = await _auctionManager.GetExpiredAuctionsCount();
            return Ok(new { expiredAuctions = count });
        }

        [HttpGet("upcoming-auctions")]
        public async Task<IActionResult> GetUpcomingAuctionsCount()
        {
            int count = await _auctionManager.GetUpcomingAuctionsCount();
            return Ok(new { upcomingAuctions = count });
        }


        [HttpGet("winners/{sellerId}")]
        public async Task<IActionResult> GetWinnersBySeller(int sellerId)
        {
            var result = await _auctionManager.GetWinnersBySeller(sellerId); 

            if (result == null || !result.Any())
                return NotFound("No winners found for this seller");

            return Ok(result);
        }

        [HttpGet("auctions-bids/{sellerId}")]
        public async Task<ActionResult> GetSellerAuctionsBids(int sellerId)
        {
            var result = await _auctionManager.GetSellerAuctionsBids(sellerId);

            if (result == null || !result.Any())
            {
                return NotFound($"No auctions with bids found for seller ID {sellerId}");
            }

            return Ok(result);
        }

        [HttpGet("{sellerId}/category-percentages")]
        public async Task<IActionResult> GetCategoryPercentages(int sellerId)
        {
            var result = await _auctionManager.GetCategoryPercentageBySeller(sellerId);

            if (result == null || !result.Any())
                return NotFound("No items or auctions found for this seller");

            return Ok(result);
        }


        [HttpGet("Get-History")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] AuctionStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _auctionManager.GetHistory(
                userId,
                status,
                page,
                pageSize);

            return Ok(result);
        }

        [HttpGet("seller/{sellerId}/category/{categoryId}/monthly-earnings")]
        public ActionResult<IEnumerable<MonthlyEarningDto>> GetMonthlyEarningsByCategory(
            int sellerId,
            int categoryId)
        {
            var earnings = _auctionManager.GetMonthlyEarningsByCategory(sellerId, categoryId);

            if (earnings == null || !earnings.Any())
                return NotFound("There are no earnings for this category or for the specified seller");

            return Ok(earnings);
        }

        [HttpGet("MostPopularProducts/{sellerId}")]
        public IActionResult GetMostPopularProducts(int sellerId, int topCount = 5)
        {
            var popularProducts = _auctionManager.GetMostPopularProductsBySeller(sellerId, topCount);

            if (popularProducts == null || !popularProducts.Any())
                return NotFound(new { Message = "No products found for this seller." });

            return Ok(popularProducts);
        }

        [HttpGet("top-profitable/{sellerId}/{categoryId}")]
        public async Task<IActionResult> GetTopProfitableAuctions(int sellerId, int categoryId)
        {
            if (sellerId <= 0 || categoryId <= 0)
                return BadRequest("SellerId and CategoryId must be greater than zero.");

            var auctions = await _auctionManager.GetTopProfitableAuctions(sellerId, categoryId);

            if (auctions == null || !auctions.Any())
                return NotFound("No profitable auctions found for this seller in this category.");

            return Ok(auctions);
        }

        [Authorize(Roles = "USER")]
        [HttpPost("report")]
        public async Task<IActionResult> ReportAuction(CreateReportDto dto)
        {
            var userId = User.FindFirst("uid")?.Value
                         ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _auctionManagerU.ReportAuctionAsync(userId, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        // ✅ 1. Get All Categories
        [HttpGet ("Get-Categories")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _category.GetAllCategoriesAsync();
            return Ok(result);
        }

        // ✅ 2. Get Category Attributes
        [HttpGet("Get-Attributes/{categoryId}")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> GetAttributes(int categoryId)
        {
            var result = await _category.GetCategoryAttributesAsync(categoryId);
            return Ok(result);
        }


        [HttpPost("Create-Auction")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> Create(CreateAuctionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _auctionManager.CreateAuction(dto, userId);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SELLER")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(int id, EditAuctionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _auctionManager.EditAuction(id, dto, userId);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SELLER")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _auctionManager.DeleteAuction(id, userId);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SELLER")]
        [HttpGet("View/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var auction = await _auctionManager.GetAuction(id,userId);

            return auction == null ? NotFound("Auction Not Found") : Ok(auction);
        }


        // [Authorize(Roles = "ADMIN")]
        [HttpGet("active")]
        public IActionResult GetActiveAuctions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var result = _auctionManager.GetActiveAuctions(page, pageSize);

            return Ok(result);
        }

        // [Authorize(Roles = "ADMIN")]
        [HttpPost("force-expire/{id}")]
        public IActionResult ForceExpireAuction(int id)
        {
            try
            {
                _auctionManager.ForceExpireAuction(id);
                return Ok(new { message = "Auction expired successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("expired")]
        public IActionResult GetExpiredAuctions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _auctionManager.GetExpiredAuctions(page, pageSize);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuctionPermanently(int id)
        {
            try
            {
                _auctionManager.DeleteAuctionPermanently(id);
                return Ok(new { message = "Auction deleted successfully" });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // [Authorize(Roles = "ADMIN")]
        [HttpGet("rejected-deleted")]
        public IActionResult GetRejectedDeletedAuctions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = _auctionManager.GetRejectedDeletedAuctions(page, pageSize);

            return Ok(result);
        }

        // [Authorize(Roles = "ADMIN")]
        [HttpDelete("permanent/{id}")]
        public IActionResult DeleteAuctionPermanentlyy(int id)
        {
            try
            {
                _auctionManager.DeleteAuctionPermanently(id);
                return Ok(new { message = "Auction deleted permanently" });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("categoryAuctions/{categoryId}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> GetAuctionsByCategory(
            int categoryId,
            [FromQuery] AuctionQueryDto queryDto,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _auctionManagerU.GetAuctionsByCategory(categoryId, userId, pageNumber, pageSize, queryDto);
            if (!result.Item1.IsSuccess)
                return BadRequest(result.Item1);

            return Ok(new
            {
                result.Item1,
                result.Item2
            });
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavoriteAuctions([FromQuery] AuctionQueryDto queryDto,int pageNumber = 1, int pageSize = 10 )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _auctionManagerU.GetFavoriteAuctions(userId, pageNumber, pageSize, queryDto);

            if (!result.Item1.IsSuccess)
                return BadRequest(result.Item1);

            return Ok(new
            {
                result.Item1,
                Data = result.Item2,
                TotalCount = result.Item3,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate()
        {
            await _auctionManagerU.CalculateHotScoresAsync();
            return Ok(new { message = "HotScore calculated successfully" });
        }

        //=========================  Auction Process =============================

        [HttpGet("User-Auction-View/{id}")]
        public async Task<IActionResult> GetAuctionById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var (auth, data) = await _auctionManagerU.GetAuctionDetails(id, userId);

            if (!auth.IsSuccess)
                return NotFound(auth);

            return Ok(new
            {
                auth,
                data
            });
        }

        [HttpPost("check-deposit/{Id}")]
        public async Task<IActionResult> CheckDeposit(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _auctionManagerU.CheckSecurityDeposit(id, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("search")]
        [Authorize(Roles = "USER")]

        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required");

            var result = await _auctionManagerU.SearchAsync(query, userId);

            return Ok(result);
        }
    }
}