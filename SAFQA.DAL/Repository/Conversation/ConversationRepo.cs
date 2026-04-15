using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Conversation
{
    public class ConversationRepo : IConversationRepo
    {
        private readonly SAFQA_Context _context;

        public ConversationRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public IQueryable<Models.Conversation> GetAll()
        {
            return _context.conversations;
        }

        public Models.Conversation GetById(int id)
        {
            return _context.conversations.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Models.Conversation conversation)
        {
            _context.conversations.Add(conversation);
            _context.SaveChanges();
        }

        public void Update(Models.Conversation conversation)
        {
            _context.conversations.Update(conversation);
            _context.SaveChanges();
        }

        public void Delete(Models.Conversation conversation)
        {
            _context.conversations.Remove(conversation);
            _context.SaveChanges();
        }
    }
}
