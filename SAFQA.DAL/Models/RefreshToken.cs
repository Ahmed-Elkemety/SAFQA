using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }   // Identity UserId يكون string
        public User User { get; set; }       // Navigation property
        public DateTime ExpiryDate { get; set; }
    }
}
