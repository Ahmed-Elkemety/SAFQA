using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Users
{
    public class UserRepo : IUserRepo
    {
        private readonly SAFQA_Context _context;
        public UserRepo(SAFQA_Context context)
        {
            _context = context;
        }


        public async Task<int> GetTotalUsers()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCount()
        {
            
            DateTime activeThreshold = DateTime.UtcNow.AddMonths(-3); 
            return await _context.Users
                .Where(u => !u.IsDeleted
                            && u.Status == UserStatus.Active 
                            && u.LastLogin >= activeThreshold)
                .CountAsync();
        }

        public async Task<int> GetBlockedUsersCount()
        {
            return await _context.Users
                .Where(u => u.Status == UserStatus.Blocked || u.IsDeleted)
                .CountAsync();
        }

        public IQueryable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(string id)
        {
            return _context.Users.FirstOrDefault(c => c.Id == id);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.City)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}