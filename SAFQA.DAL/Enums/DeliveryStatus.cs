using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Enums
{
    public enum DeliveryStatus
    {
        Pending = 1,
        Shipped = 2,
        InTransit = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Failed = 6,
        Returned = 7
    }
}
