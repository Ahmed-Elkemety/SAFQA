using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.TrackingDto
{
    public class OrderTrackingDto
    {
        public int AuctionId { get; set; }

        public string ProductTitle { get; set; }
        public byte[] ProductImage { get; set; }

        public string TrackingId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public List<OrderStatusStepDto> StatusSteps { get; set; }
    }
}