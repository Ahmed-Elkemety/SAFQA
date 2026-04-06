using SAFQA.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.WalletDto
{
    public class TransactionHistoryDto
    {
        public DateTime Date { get; set; }       
        public string Type { get; set; }         
        public decimal Amount { get; set; }      
        public int Day => Date.Day;            
        public int Month => Date.Month;         
        public int Hour => Date.Hour;           
        public int Minute => Date.Minute;        
    }
}
