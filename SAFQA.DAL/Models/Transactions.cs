using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt{ get; set; }
        public int? WalletId { get; set; }
        public Wallet Wallet { get; set; }
    }
}
