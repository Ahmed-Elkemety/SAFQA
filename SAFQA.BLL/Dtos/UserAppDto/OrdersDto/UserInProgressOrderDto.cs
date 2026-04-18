using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.OrdersDto
{
    public class UserInProgressOrderDto
    {
        public int AuctionId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public List<byte[]> Images { get; set; }
    }
}
