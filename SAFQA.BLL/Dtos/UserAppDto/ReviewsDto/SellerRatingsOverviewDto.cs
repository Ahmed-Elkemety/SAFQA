using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.ReviewsDto
{
    public class SellerRatingsOverviewDto
    {
        public int SellerId { get; set; }
        public float AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<SellerReviewDto> Reviews { get; set; }
    }
}
