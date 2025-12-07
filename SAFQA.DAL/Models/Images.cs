using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class Images
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public bool IsMain { get; set; }
        public int ItemId { get; set; }
        public Item item { get; set; }
    }
}
