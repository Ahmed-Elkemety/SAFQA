using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.RepoDtos.UserApp.Home.TrendingAuction
{
    public class TrendingAuction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte[] Image { get; set; }
    }
}
