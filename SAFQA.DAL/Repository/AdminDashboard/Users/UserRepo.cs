using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.AdminDashboard.Users
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
    }
}