using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class SavedCard
    {
        public int Id { get; set; }

        public string Last4Digits { get; set; }

        public string CardBrand { get; set; } 

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public string CardholderName { get; set; }

        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
