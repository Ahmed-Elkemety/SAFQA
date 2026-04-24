using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.RepoDtos.UserApp.Home.Categorys
{
    public class Categorys
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public int AuctionCount { get; set; }
    }
}
