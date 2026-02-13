using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class PendingUserRegistration
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // هش الباسورد
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public int CityId { get; set; }
        public string OtpHash { get; set; }
        public DateTime OtpExpiration { get; set; }
        public bool IsUsed { get; set; } = false;
    }

}
