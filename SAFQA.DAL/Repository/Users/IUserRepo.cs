using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Users
{
    public interface IUserRepo
    {
        Task<int> GetTotalUsers();
        Task<int> GetActiveUsersCount();
        Task<int> GetBlockedUsersCount();

        IQueryable<User> GetAll();
        User GetById(string id);
        void Add(User user);
        void Update(User user);
        void Delete(User user);
    }
}
