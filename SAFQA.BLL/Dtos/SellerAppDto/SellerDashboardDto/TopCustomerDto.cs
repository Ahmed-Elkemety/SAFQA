using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class TopCustomerDto
    {
        public string Name { get; set; }
        public string CompanyName { get; set; } 
        public string Email { get; set; }
        public int ParticipatedAuctions { get; set; }
        public decimal TotalPaid { get; set; }
    }
}
