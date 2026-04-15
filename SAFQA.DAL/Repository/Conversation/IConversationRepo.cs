using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Conversation
{
    public interface IConversationRepo
    {
        IQueryable<Models.Conversation> GetAll();
        Models.Conversation GetById(int id);
        void Add(Models.Conversation conversation);
        void Update(Models.Conversation conversation);
        void Delete(Models.Conversation conversation);
    }
}