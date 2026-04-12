using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class SellerDetailsDto
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string CityName { get; set; }
        public byte[]? StoreLogo { get; set; }

        public byte[]? CRDocument { get; set; }
        public string? TaxId { get; set; }
        public byte[]? OwnerIdDocument { get; set; }

        public string VerificationStatus { get; set; }
        public string StoreStatus { get; set; }
    }
}
