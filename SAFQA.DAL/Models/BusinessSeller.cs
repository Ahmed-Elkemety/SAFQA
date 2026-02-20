using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class BusinessSeller
{
    public int Id { get; set; }

    public int SellerId { get; set; }
    public Seller Seller { get; set; }

    // Legal Documents
    public byte[] CommercialRegister { get; set; }
    public byte[] TaxId { get; set; }
    public byte[] OwnerNationalIdFront { get; set; }
    public byte[] OwnerNationalIdBack { get; set; }

    // Financial Details
    public string BankName { get; set; }
    public string AccountName { get; set; }
    public string IBAN { get; set; }
    public string? LocalAccountNumber { get; set; }
}
}
