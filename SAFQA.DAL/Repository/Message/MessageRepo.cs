using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Message
{
    public class MessageRepo : IMessageRepo
    {
        private readonly SAFQA_Context _context;

        public MessageRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public IQueryable<Models.Message> GetAll()
        {
            return _context.Messages;
        }

        public Models.Message GetById(int id)
        {
            return _context.Messages.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Models.Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
        }

        public void Update(Models.Message message)
        {
            _context.Messages.Update(message);
            _context.SaveChanges();
        }

        public void Delete(Models.Message message)
        {
            _context.Messages.Remove(message);
            _context.SaveChanges();
        }
    }
}
