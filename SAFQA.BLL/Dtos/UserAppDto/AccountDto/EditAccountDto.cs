using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SAFQA.DAL.Enums;

namespace SAFQA.BLL.Dtos.UserAppDto.AccountDto
{
    public class EditAccountDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public int CityId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
