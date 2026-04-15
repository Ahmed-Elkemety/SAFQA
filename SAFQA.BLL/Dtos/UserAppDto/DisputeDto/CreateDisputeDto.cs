using SAFQA.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.DisputeDto
{
    public class CreateDisputeDto
    {
        public int AuctionId { get; set; }
        public string Description { get; set; }
        public DisputeResolutionType ResolutionType { get; set; }
        public List<string> Evidences { get; set; }
    }
}
