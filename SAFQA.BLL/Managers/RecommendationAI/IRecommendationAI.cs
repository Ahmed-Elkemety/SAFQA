using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.RecommendationAI
{
    public interface IRecommendationAI
    {
        Task<List<RecommendationResult>> GetRecommendationsAsync(string userId);
    }
}
