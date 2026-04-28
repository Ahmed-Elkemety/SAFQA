using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.BLL.Dtos.SellerAppDto.CategoryDto
{
    public class CategoryAttributeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DataTypes DataType { get; set; }
        public byte[] Image { get; set; }

        public Unit Unit { get; set; }
        public bool IsRequired { get; set; }
    }
}
