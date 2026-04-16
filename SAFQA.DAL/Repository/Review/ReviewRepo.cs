using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Review
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly SAFQA_Context _context;
        public ReviewRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public IQueryable<Models.Review> GetAll()
        {
            return _context.Reviews;
        }

        public async Task<Models.Review> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Models.Review> GetByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task AddAsync(Models.Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Models.Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Models.Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}