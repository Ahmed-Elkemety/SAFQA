using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Message
{
    public interface IMessageRepo
    {
        IQueryable<Models.Message> GetAll();
        Models.Message GetById(int id);
        void Add(Models.Message message);
        void Update(Models.Message message);
        void Delete(Models.Message message);
    }
}
