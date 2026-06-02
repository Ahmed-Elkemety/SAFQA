using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.DisputeDto
{
    public class EscalateCardDTO
    {
        public int DisputeId { get; set; }

        public string AuctionTitle { get; set; }

        public string SellerName { get; set; }

        public string BuyerName { get; set; }

        public decimal PaidAmount { get; set; }

        public List<byte[]> Evidences { get; set; }
    }
}