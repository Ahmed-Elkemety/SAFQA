using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CodeHash { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsUsed { get; set; }
    }
}
