using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Enums;

namespace SAFQA.BLL.Dtos.AccountDto
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public UserLanguage Language { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
