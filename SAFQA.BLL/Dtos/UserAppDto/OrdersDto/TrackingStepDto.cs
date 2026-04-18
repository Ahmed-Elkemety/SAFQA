using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.OrdersDto
{
    public class TrackingStepDto
    {
        public string step { get; set; }
        public DateTime? Date { get; set; }
        public bool IsCompleted { get; set; }
        
    }
}