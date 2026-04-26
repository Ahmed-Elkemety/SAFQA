using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.ProxyBidding
{
    public class CreateProxyDto
    {
        public int AuctionId { get; set; }
        public int Max { get; set; }
        public int Step { get; set; }
    }
}
