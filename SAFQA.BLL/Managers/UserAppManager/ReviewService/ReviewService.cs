using SAFQA.BLL.Dtos.UserAppDto.ReviewsDto;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Review;
using SAFQA.DAL.Repository.Seller;
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

        public ReviewService(
            IAuctionRepository auctionRepo,
            IReviewRepo reviewRepo,
            IsellerRepo sellerRepo)
        {
            _auctionRepo = auctionRepo;
            _reviewRepo = reviewRepo;
            _sellerRepo = sellerRepo;
        }

        public async Task AddReviewAsync(string userId, AddReviewDto dto)
        {

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new Exception("Rating must be between 1 and 5");


            var auction = await _auctionRepo.GetByIdAsync(dto.AuctionId);
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
                DeliverySpeed = dto.DeliverySpeed,
                accurateDescription = dto.AccurateDescription,

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
                return;

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


        public async Task<IQueryable<Review>> GetSellerReviewsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var seller = await _sellerRepo.GetByUserIdAsync(userId);

            if (seller == null)
                throw new Exception("Seller not found");

            return _reviewRepo
                .GetAll()
                .Where(r => r.SellerId == seller.Id);
        }
    }
}