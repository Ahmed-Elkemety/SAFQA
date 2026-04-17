using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAFQA.BLL.Dtos.UserAppDto.AccountDto
{
    public class UserAccountDto
    {
        public byte[]? Image { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string City { get; set; }
    }
}
