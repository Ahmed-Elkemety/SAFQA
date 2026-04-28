using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class UserFollowers
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int SellerId { get; set; }

        public virtual Seller Seller { get; set; }

        public DateTime FollowedAt { get; set; }
    }
}
