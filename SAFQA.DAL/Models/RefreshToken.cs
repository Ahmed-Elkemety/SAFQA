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
        public string TokenHash { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DeviceId { get; set; }
        public bool IsRevoked { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
