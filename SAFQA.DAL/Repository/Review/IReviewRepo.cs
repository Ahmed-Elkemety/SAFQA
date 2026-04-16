using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Review
{
    public interface IReviewRepo
    {
        IQueryable<Models.Review> GetAll();
        Task<Models.Review> GetByIdAsync(int id);
        Task<Models.Review> GetByUserIdAsync(string userId);
        Task AddAsync(Models.Review review);
        Task UpdateAsync(Models.Review review);
        Task DeleteAsync(Models.Review review);
    }
}