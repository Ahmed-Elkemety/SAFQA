using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string Description { get; set; }
        public ItemCondition Condition { get; set; }
        public string WarrantyInfo { get; set; }
        public int AuctionId { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Auction Auction { get; set; }
        public ICollection<Images> images { get; set; }
        public ICollection<ItemAttributesValue> itemAttributesValues { get; set; }


    }
}
