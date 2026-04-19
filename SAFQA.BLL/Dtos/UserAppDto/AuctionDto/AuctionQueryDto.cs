using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.BLL.Dtos.UserAppDto.AuctionDto
{
    public class AuctionQueryDto
    {
        public int? CategoryId { get; set; }
        public List<AuctionStatus>? Statuses { get; set; }
        public List<int>? CityIds { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public AuctionSortBy SortBy { get; set; }
    }
}
