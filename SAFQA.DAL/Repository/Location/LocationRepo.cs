using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.Location
{
    public class LocationRepo : ILocationRepo
    {
        private readonly SAFQA_Context _context;

        public LocationRepo(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            return await _context.countries
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<City>> GetCitiesByCountryIdAsync(int countryId)
        {
            return await _context.cities
                .Where(c => c.CountryId == countryId)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
