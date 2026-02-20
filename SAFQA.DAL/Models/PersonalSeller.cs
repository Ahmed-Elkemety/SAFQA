using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class PersonalSeller
    {
        public int Id { get; set; }

        public int SellerId { get; set; }
        public Seller Seller { get; set; }

        public byte[] NationalIdFront { get; set; }
        public byte[] NationalIdBack { get; set; }
        public byte[] SelfieWithId { get; set; }
    }
}
