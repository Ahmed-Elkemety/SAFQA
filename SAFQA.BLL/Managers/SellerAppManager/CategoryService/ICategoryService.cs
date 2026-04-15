using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.SellerAppDto.CategoryDto;

namespace SAFQA.BLL.Managers.SellerAppManager.CategoryService
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<List<CategoryAttributeDTO>> GetCategoryAttributesAsync(int categoryId);
    }
}
