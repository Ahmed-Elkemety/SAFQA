using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Dispute
{
    public interface IDiputeRepo
    {
        IQueryable<Disputes> GetAll();
        Disputes GetById(int id);
        void Add(Disputes disputes);
        void Update(Disputes disputes);
        void Delete(Disputes disputes);
        Task<List<Disputes>> GetUserDisputesAsync(string userId);

    }
}
