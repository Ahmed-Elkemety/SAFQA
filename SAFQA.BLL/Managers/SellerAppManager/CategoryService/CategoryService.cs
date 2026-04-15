using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.SellerAppDto.CategoryDto;
using SAFQA.DAL.Database;
using SAFQA.DAL.Repository.Category;

namespace SAFQA.BLL.Managers.SellerAppManager.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IcategoryRepo _repo;
        private readonly SAFQA_Context _context;

        public CategoryService(IcategoryRepo repo , SAFQA_Context context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _repo.GetAllAsync();

            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<List<CategoryAttributeDTO>> GetCategoryAttributesAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Invalid CategoryId");

            var exists = await _context.Category.AnyAsync(c => c.Id == categoryId); ;

            if (!exists)
                throw new Exception("Category not found");

            var attrs = await _repo.GetAttributesByCategoryIdAsync(categoryId);

            if (!attrs.Any())
                throw new Exception("This category has no attributes");

            return attrs.Select(a => new CategoryAttributeDTO
            {
                Id = a.Id,
                Name = a.Name,
                DataType = a.dataType,
                Unit = a.unit,
                IsRequired = a.IsRequird
            }).ToList();
        }
    }
}
