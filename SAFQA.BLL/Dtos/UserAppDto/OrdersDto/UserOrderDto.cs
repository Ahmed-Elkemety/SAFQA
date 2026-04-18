using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.OrdersDto
{
    public class UserOrderDto
    {
        public int AuctionId { get; set; }
        public DateTime DeliveredAt { get; set; }
        public List<byte[]> Images { get; set; }
    }
}
