using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System.Linq;

namespace SAFQA.DAL.Repository.Wallet
{
    public class CardRepo : ICardRepo
    {
        private readonly SAFQA_Context _context;

        public CardRepo(SAFQA_Context context)
        {
            _context = context;
        }

        public IQueryable<SavedCard> GetAll()
        {
            return _context.savedCards.AsQueryable();
        }

        public SavedCard GetById(int id)
        {
            return _context.savedCards.FirstOrDefault(c => c.Id == id);
        }

        public void Add(SavedCard card)
        {
            _context.savedCards.Add(card);
            _context.SaveChanges();
        }

        public void Update(SavedCard card)
        {
            _context.savedCards.Update(card);
            _context.SaveChanges();
        }

        public void Delete(SavedCard card)
        {
            _context.savedCards.Remove(card);
            _context.SaveChanges();
        }
    }
}