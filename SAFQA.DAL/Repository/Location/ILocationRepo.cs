using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.Location
{
    public interface ILocationRepo
    {
        Task<List<Country>> GetCountriesAsync();
        Task<List<City>> GetCitiesByCountryIdAsync(int countryId);
    }
}
