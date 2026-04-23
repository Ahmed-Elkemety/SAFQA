using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.DisputeDto
{
    public class DisputeTrackingDto
    {
        public int DisputeId { get; set; }
        public string Status { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool CanChat { get; set; }
        public bool CanEscalate { get; set; }
        public bool CanCancel { get; set; }
    }
}
