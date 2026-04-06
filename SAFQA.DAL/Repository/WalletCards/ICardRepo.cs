using SAFQA.DAL.Models;
using System.Linq;

namespace SAFQA.DAL.Repository.Wallet
{
    public interface ICardRepo
    {
        IQueryable<SavedCard> GetAll();
        SavedCard GetById(int id);
        void Add(SavedCard card);
        void Update(SavedCard card);
        void Delete(SavedCard card);
    }
}