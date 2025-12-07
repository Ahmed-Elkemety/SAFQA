using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class CategoryAttributes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DataTypes dataType { get; set; }
        public Unit unit { get; set; }
        public bool IsRequird { get; set; }

        public int CategoryId { get; set; }
        public Category category { get; set; }
        public ICollection<ItemAttributesValue> itemAttributesValues { get; set; }

    }
}
