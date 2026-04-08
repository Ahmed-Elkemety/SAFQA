using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class PopularProductsDto
    {
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public string Description { get; set; }
    }
}
