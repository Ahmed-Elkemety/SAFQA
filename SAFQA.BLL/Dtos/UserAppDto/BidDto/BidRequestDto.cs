using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.BidDto
{
    public class BidRequestDto
    {
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
    }
}
