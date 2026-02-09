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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime Expiration { get; set; }

        public bool IsUsed { get; set; }

        public int Attempts { get; set; } = 0;

        public User User { get; set; }
    }
}
