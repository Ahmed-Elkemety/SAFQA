using SAFQA.BLL.Dtos.UserAppDto.ReviewsDto;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.ReviewService
{
    public interface IReviewService
    {
        Task AddReviewAsync(string userId, AddReviewDto dto);
        Task<IQueryable<Review>> GetSellerReviewsAsync(string userId); // if User Want To See Review About Specific Seller 


    }
}