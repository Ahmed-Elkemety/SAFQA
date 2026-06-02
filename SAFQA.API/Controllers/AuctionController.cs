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
        [Authorize(Roles = "SELLER")]
        [HttpGet("Total_active")]
        public async Task<IActionResult> GetActiveAuctions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var activeAuctions = await _auctionManager.GetActiveSellerAuctions(userId);
            return Ok(activeAuctions);
        }


        // GET: api/Auction/total/5
        [Authorize(Roles = "SELLER")]
        [HttpGet("total/Seller/Auctions")]
        public async Task<IActionResult> GetTotalAuctions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var total = await _auctionManager.GetTotalSellerAuctions(userId);
            return Ok(total);
        }

        [HttpGet("Seller/Top customers")]
        public async Task<IActionResult> GetSellerCustomers()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            var result = await _auctionManager.GetTopCustomers(UserId);

            return Ok(result);
        }

        [HttpGet("total-auctions")]
        public async Task<IActionResult> GetTotalAuction()
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

        [HttpGet("winners")]
        public async Task<IActionResult> GetWinnersBySeller()
        {
            var sellerUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(sellerUserId))
                return Unauthorized();

            var result = await _auctionManager.GetWinnersBySeller(sellerUserId); 

            if (result == null || !result.Any())
                return NotFound("No winners found for this seller");

            return Ok(result);
        }

        [HttpGet("auctions-bids")]
        public async Task<ActionResult> GetSellerAuctionsBids()
        {
            var sellerUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(sellerUserId))
                return Unauthorized();

            var result = await _auctionManager.GetSellerAuctionsBids(sellerUserId);

            if (result == null || !result.Any())
            {
                return NotFound("No auctions with bids found for this seller");
            }

            return Ok(result);
        }

        [HttpGet("category-percentages")]
        public async Task<IActionResult> GetCategoryPercentages()
        {
            var sellerUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(sellerUserId))
                return Unauthorized();

            var result = await _auctionManager
                .GetCategoryPercentageBySeller(sellerUserId);

            if (!result.Any())
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

        [HttpGet("monthly-earnings/{categoryId}")]
        public IActionResult GetMonthlyEarningsByCategory(int categoryId)
        {
            var sellerUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(sellerUserId))
                return Unauthorized();

            var result = _auctionManager
                .GetMonthlyEarningsByCategory(sellerUserId, categoryId);

            if (!result.Any())
                return NotFound("No earnings found.");

            return Ok(result);
        }

        [HttpGet("MostPopularProducts")]
        public IActionResult GetMostPopularProducts(int topCount = 5)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var popularProducts =
                _auctionManager.GetMostPopularProductsBySeller(userId, topCount);

            if (popularProducts == null || !popularProducts.Any())
                return NotFound(new
                {
                    Message = "No products found for this seller."
                });

            return Ok(popularProducts);
        }

        [HttpGet("top-profitable/{categoryId}")]
        public async Task<IActionResult> GetTopProfitableAuctions(int categoryId)
        {
            if (categoryId <= 0)
                return BadRequest("CategoryId must be greater than zero.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var auctions = await _auctionManager.GetTopProfitableAuctions(userId, categoryId);

            if (auctions == null || !auctions.Any())
                return NotFound("No profitable auctions found.");

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
 

        [HttpGet ("Get-Categories")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _category.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("Get-Attributes/{categoryId}")]

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

        [Authorize(Roles = "USER")]
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

        public async Task<IActionResult> Search([FromQuery] AuctionQueryDto queryDto, string query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required");

            var result = await _auctionManagerU.SearchAsync(query, userId, queryDto);

            return Ok(new
            {
                result.Item1,
                Data = result.Item2
            }); ;
        }
    }
}