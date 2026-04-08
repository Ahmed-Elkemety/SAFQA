using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Repository.Auction;
using static SAFQA.BLL.Help.Helper;

namespace SAFQA.BLL.Managers.SellerAppManager.AuctionManager
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepo _repo;

        public AuctionService(IAuctionRepo repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<SellerActionHistoryDto>> GetHistory(
            string userId,
            AuctionStatus? status,
            int page,
            int pageSize)
        {
            var query = _repo.GetSellerAuctions(userId);

            var mappedQuery = query.Select(a => new SellerActionHistoryDto
            {
                AuctionId = a.Id,
                Title = a.Title,

                DisplayPrice =
                a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled ? a.StartingPrice :
                a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon ? a.CurrentPrice :
                a.Status == AuctionStatus.Finished ? a.FinalPrice :
                0,

                DisplayDate =
                a.Status == AuctionStatus.Upcoming || a.Status == AuctionStatus.Cancelled ? a.StartDate :
                a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndingSoon ? a.EndDate :
                a.Status == AuctionStatus.Finished ? a.EndDate :
                a.StartDate,

                TotalBids = a.TotalBids,
                Status = a.Status,

                Image = a.Image != null
                    ? Convert.ToBase64String(a.Image)
                    : null
            });

            if (status.HasValue)
            {
                mappedQuery = mappedQuery
                    .Where(x => x.Status == status.Value);
            }

            var totalCount = await mappedQuery.CountAsync();

            var data = await mappedQuery
                .OrderByDescending(x => x.DisplayDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<SellerActionHistoryDto>
            {
                Data = data,
                CurrentPage = page,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasNextPage = page * pageSize < totalCount
            };
        }


    }
}
