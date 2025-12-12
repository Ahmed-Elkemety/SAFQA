using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Transactions> Transactions { get; set; } 
    }
}
