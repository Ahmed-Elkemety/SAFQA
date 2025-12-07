using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class ItemAttributesValue
    {
        public int Id { get; set; }
        public string value { get; set; }
        public int CategoryAttributeId { get; set; }
        public CategoryAttributes categoryAttributes { get; set; }
        public int ItemId { get; set; }
        public Item MyProperty { get; set; }
    }
}
