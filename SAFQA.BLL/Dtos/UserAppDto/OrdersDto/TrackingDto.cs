using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.OrdersDto
{
    public class TrackingDto
    {
        public int AuctionId { get; set; }
        public List<string> Images { get; set; }
        public List<TrackingStepDto> Steps { get; set; }
    }
}
