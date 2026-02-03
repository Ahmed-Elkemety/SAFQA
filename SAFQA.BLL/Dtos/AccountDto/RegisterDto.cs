using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Enums;

namespace SAFQA.BLL.Dtos.AccountDto
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }
        public UserLanguage Language { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int cityId { get; set; }
    }
}
