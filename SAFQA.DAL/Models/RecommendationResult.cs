using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class RecommendationResult
    {
        public string AuctionId { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public double Match_Score { get; set; }
    }
}
