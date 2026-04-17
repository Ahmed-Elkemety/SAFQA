using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.ReviewsDto
{
    public class AddReviewDto
    {
        public int AuctionId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int DeliverySpeed { get; set; }
        public int AccurateDescription { get; set; }
    }
}