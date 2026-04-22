using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.BLL.Dtos.UserAppDto.DisputeDto
{
    public class DisputeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DisputeStatus Status { get; set; }
        public DisputeProblemType ProblemType { get; set; }
        public string? Description { get; set; }
        public DisputeResolutionType ResolutionType { get; set; }
        public List<byte[]> Evidences { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }

        public int AuctionId { get; set; }
        public string? AuctionTitle { get; set; }    }
}
