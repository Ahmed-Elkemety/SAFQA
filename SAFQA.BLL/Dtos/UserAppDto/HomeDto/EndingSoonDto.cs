using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.HomeDto
{
    public class EndingSoonDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte[]? Image { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndDate { get; set; }
    }
}
