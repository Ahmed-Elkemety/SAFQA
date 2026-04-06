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

        // آخر 4 أرقام بس
        public string Last4Digits { get; set; }

        public string CardBrand { get; set; } // Visa - MasterCard

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        // Token جاي من الـ Payment Provider
        public string PaymentToken { get; set; }

        public bool IsDefault { get; set; } = false;

        // العلاقة بالـ Wallet
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
