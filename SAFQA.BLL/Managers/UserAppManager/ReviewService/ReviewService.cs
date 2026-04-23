using SAFQA.BLL.Dtos.UserAppDto.ReviewsDto;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Review;
using SAFQA.DAL.Repository.Seller;
using SAFQA.DAL.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IAuctionRepository _auctionRepo;
        private readonly IReviewRepo _reviewRepo;
        private readonly IsellerRepo _sellerRepo;
        private readonly IUserRepo _userRepo;

        public ReviewService(
            IAuctionRepository auctionRepo,
            IReviewRepo reviewRepo,
            IsellerRepo sellerRepo,
            IUserRepo userRepo)
        {
            _auctionRepo = auctionRepo;
            _reviewRepo = reviewRepo;
            _sellerRepo = sellerRepo;
            _userRepo = userRepo;
        }

        public async Task AddReviewAsync(string userId, AddReviewDto dto)
        {

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new Exception("Rating must be between 1 and 5");


            var auction = await _auctionRepo.GetAuctionWithDeliveryAsync(dto.AuctionId);
            if (auction == null)
                throw new Exception("Auction not found");


            if (auction.WinnerUserId != userId)
                throw new Exception("You are not allowed to review this auction");

   
            var delivery = auction.delivery;
            
            if (delivery == null || delivery.Status != DeliveryStatus.Deliverd)
                throw new Exception("You can only review after delivery");

       
            var existingReview = _reviewRepo
                .GetAll()
                .FirstOrDefault(r => r.AuctionId == dto.AuctionId && r.UserId == userId);

            if (existingReview != null)
                throw new Exception("You already reviewed this auction");

     
            var review = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                Date = DateTime.UtcNow,
                UserId = userId,
                AuctionId = dto.AuctionId,
                SellerId = auction.SellerId
            };


            await _reviewRepo.AddAsync(review);
            
            await UpdateSellerRatingAsync(auction.SellerId.Value);
        }

        private async Task UpdateSellerRatingAsync(int sellerId)
        {
            var seller = await _sellerRepo.GetByIdAsync(sellerId);

            if (seller == null)
                throw new Exception("Seller not found");

            var reviews = _reviewRepo.GetAll()
                .Where(r => r.SellerId == sellerId);

            if (reviews.Any())
            {
                seller.Rating = (float)reviews.Average(r => r.Rating);
            }
            else
            {
                seller.Rating = 0;
            }

            _sellerRepo.Update(seller);
            await _sellerRepo.SaveChangesAsync();
        }


        public SellerRatingsOverviewDto GetSellerReviews(int sellerId)
        {
            var seller = _sellerRepo.GetAll()
                .FirstOrDefault(s => s.Id == sellerId && !s.IsDeleted);

            if (seller == null)
                return null;

            var reviews = _reviewRepo.GetAll()
                .Where(r => r.SellerId == sellerId)
                .ToList();

            var userIds = reviews.Select(r => r.UserId).Distinct().ToList();

            var users = _userRepo.GetAll()
                .Where(u => userIds.Contains(u.Id))
                .ToList();

            var reviewDtos = reviews.Select(r =>
            {
                var user = users.FirstOrDefault(u => u.Id == r.UserId);

                return new SellerReviewDto
                {
                    UserName = user?.FullName ?? "Unknown",
                    UserImage = user?.Image,

                    Rating = r.Rating,
                    Comment = r.Comment,
                    Date = r.Date
                };
            }).ToList();

            return new SellerRatingsOverviewDto
            {
                SellerId = seller.Id,
                AverageRating = reviews.Any() ? (float)reviews.Average(r => r.Rating) : 0,
                TotalReviews = reviews.Count,
                Reviews = reviewDtos
            };
        }
    }
}