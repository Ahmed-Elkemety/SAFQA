using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.RepoDtos.UserApp.Home.CategoryWithDetails;

namespace SAFQA.DAL.Repository.Category
{
    public interface IcategoryRepo
    {
        Task<List<CategoryWithDetails>> GetCategoriesWithDetailsAsync();
    }
}
