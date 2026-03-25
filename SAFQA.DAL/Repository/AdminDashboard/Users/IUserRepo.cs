using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.AdminDashboard.Users
{
    public interface IUserRepo
    {
        Task<int> GetTotalUsers();
        Task<int> GetActiveUsersCount();
        Task<int> GetBlockedUsersCount();
    }
}
