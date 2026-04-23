using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.ReviewsDto
{
    public class SellerReviewDto
    {
        public string UserName { get; set; }
        public byte[]? UserImage { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        public DateTime Date { get; set; }
    }
}
