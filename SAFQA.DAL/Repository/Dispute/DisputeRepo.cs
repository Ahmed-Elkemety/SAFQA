using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Dispute
{
    public class DisputeRepo : IDiputeRepo
    {
        private readonly SAFQA_Context _context;

        public DisputeRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public IQueryable<Disputes> GetAll()
        {
            return _context.Disputes;
        }

        public Disputes GetById(int id)
        {
            return _context.Disputes.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Disputes disputes)
        {
            _context.Disputes.Add(disputes);
            _context.SaveChanges();
        }

        public void Update(Disputes disputes)
        {
            _context.Disputes.Update(disputes);
            _context.SaveChanges();
        }

        public void Delete(Disputes disputes)
        {
            _context.Disputes.Remove(disputes);
            _context.SaveChanges();
        }

        public async Task<List<Disputes>> GetUserDisputesAsync(string userId)
        {
            return await _context.Disputes
                .Include(d => d.Auction)
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }
    }
}
