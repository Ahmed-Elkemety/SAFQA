using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.BLL.Dtos.DeliveryDto
{
    public class DeliveryDto
    {
        public int Id { get; set; }
        public DeliveryStatus Status { get; set; }

        public string UserNumber { get; set; }
        public string UserEmail { get; set; }

        public string AuctionTitle { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
