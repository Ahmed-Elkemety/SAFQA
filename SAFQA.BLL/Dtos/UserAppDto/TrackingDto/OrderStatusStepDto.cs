using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.TrackingDto
{
    public class OrderStatusStepDto
    {
        public string StepName { get; set; }   // Order Placed / In Progress / Shipping / Delivered
        public DateTime? Date { get; set; }
        public bool IsCompleted { get; set; }
    }
}
