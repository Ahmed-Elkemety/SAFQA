using SAFQA.DAL.Models;
using SAFQA.DAL.RepoDtos.UserApp.Home.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Category
{
    public interface IcategoryRepo
    {
        IQueryable<Models.Category> GetAll();
        Models.Category GetById(int Id);
        void Add(Models.Category category);
        void Update(Models.Category category);
        void Delete(Models.Category category);
        Task<List<Models.Category>> GetAllAsync();
        Task<List<CategoryAttributes>> GetAttributesByCategoryIdAsync(int categoryId);
        Task<List<Categorys>> GetCategoriesWithCountAsync();


    }
}