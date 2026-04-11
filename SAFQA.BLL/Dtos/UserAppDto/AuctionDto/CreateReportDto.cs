using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.AuctionDto
{
    public class CreateReportDto
    {
        public int AuctionId { get; set; }
        public string Reason { get; set; }
        public string? Notes { get; set; }
    }
}
