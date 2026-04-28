using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.RepoDtos.UserApp.Home.Categorys;
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

        public async Task<List<Models.Category>> GetAllAsync()
        {
            return await _context.Category
                .Where(c => c != null)
                .ToListAsync();
        }

        public async Task<List<CategoryAttributes>> GetAttributesByCategoryIdAsync(int categoryId)
        {
            return await _context.categoryAttributes
                .Where(a => a.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<Categorys>> GetCategoriesWithCountAsync()
        {
            return await _context.Category
             .AsNoTracking()
             .Select(c => new Categorys
             {
                 Id = c.Id,
                 Name = c.Name,
                 Image = c.Image,
                 AuctionCount = c.Auctions.Count()
             })
             .ToListAsync();
        }
    }
}
