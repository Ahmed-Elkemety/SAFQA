using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class PendingSellerDto
    {
        public string UserId { get; set; }
        public string BusinessName { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
    }
}
