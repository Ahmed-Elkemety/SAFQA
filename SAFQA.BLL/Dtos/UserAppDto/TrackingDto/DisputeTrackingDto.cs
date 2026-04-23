using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.TrackingDto
{
    public class DisputeTrackingDto
    {
        public int DisputeId { get; set; }
        public string Status { get; set; }
        public int RemainingDays { get; set; }
        public int RemainingHours { get; set; }
        public int RemainingMinutes { get; set; }
        public bool CanChatWithSeller { get; set; }
        public bool CanEscalate { get; set; }
        public bool CanCancel { get; set; }
    }
}