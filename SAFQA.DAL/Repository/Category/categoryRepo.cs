using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.RepoDtos.UserApp.Home.CategoryWithDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IQueryable<Models.Category> GetAll()
        {
            return _context.Category;
        }
        public Models.Category GetById(int id)
        {
            return _context.Category.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Models.Category category)
        {
            _context.Category.Add(category);
            _context.SaveChanges();
        }

        public void Update(Models.Category category)
        {
            _context.Category.Update(category);
            _context.SaveChanges();
        }

        public void Delete(Models.Category category)
        {
            _context.Category.Remove(category);
            _context.SaveChanges();
        }
    }
}
