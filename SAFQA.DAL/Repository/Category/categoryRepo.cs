using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.RepoDtos.UserApp.Home.CategoryWithDetails;

namespace SAFQA.DAL.Repository.Category
{
    public class categoryRepo : IcategoryRepo
    {
        private readonly SAFQA_Context _context;

        public categoryRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public async Task<List<CategoryWithDetails>> GetCategoriesWithDetailsAsync()
        {
            return await _context.Category
                .Select(c => new CategoryWithDetails
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = c.Image,
                    ItemCount = c.Items.Count()
                })
                .ToListAsync();
        }
    }
}
