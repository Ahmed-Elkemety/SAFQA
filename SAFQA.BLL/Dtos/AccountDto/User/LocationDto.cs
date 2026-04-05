using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.AccountDto.User
{
    public class LocationDto
    {
        public class CountryDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class CityDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
