using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using SAFQA.BLL.Help;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.UserAppManager.ChatService;
using SAFQA.BLL.Managers.UserAppManager.ConversationService;
using SAFQA.BLL.Managers.UserAppManager.NotificationService;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Delivery;
using SAFQA.DAL.Repository.Dispute;
using SAFQA.DAL.Repository.Seller;
using SAFQA.DAL.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;

namespace SAFQA.BLL.Managers.UserAppManager.DisputeService
{
    public class DisputeService : IDisputeService
    {
        private readonly IDiputeRepo _disputeRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IUserRepo _userRepo;
        private readonly IsellerRepo _sellerRepo;
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;
        private readonly IDeliveryRepo _deliveryRepo;
        private readonly IHubContext<ChatHub> _hub;
        public DisputeService(
            IDiputeRepo disputeRepo,
            IAuctionRepository auctionRepo,
            IUserRepo userRepo,
            IsellerRepo sellerRepo,
            IChatService chatService,
            IDeliveryRepo deliveryRepo,
            INotificationService notificationService,
            IHubContext<ChatHub> hub)
        {
            _disputeRepo = disputeRepo;
            _auctionRepo = auctionRepo;
            _userRepo = userRepo;
            _sellerRepo = sellerRepo;
            _chatService = chatService;
            _deliveryRepo = deliveryRepo;
            _hub = hub;
            _notificationService = notificationService;
        }

        public async Task<ConversationDto> CreateDispute(string userId, CreateDisputeDto dto)
        {

            var auction = _auctionRepo.GetById(dto.AuctionId);
            if (auction == null)
                throw new Exception("Auction not found");

            if (auction.SellerId == null)
                throw new Exception("This auction has no seller");

            var existingDispute = _disputeRepo.GetAll()
                   .FirstOrDefault(d =>
                       d.AuctionId == dto.AuctionId &&
                       d.UserId == userId &&
                       !d.IsDeleted);

            if (existingDispute != null)
            {
                throw new Exception(
                    $"You already created a dispute for this auction. Dispute Id: {existingDispute.Id}"
                );
            }


            var delivery = await _deliveryRepo.GetByAuctionIdAsync(auction.Id);

            if (delivery == null)
                throw new Exception("No delivery found for this auction");

            var user = _userRepo.GetById(userId);
            if (user == null)
                throw new Exception("User not found");

            var seller = await _sellerRepo.GetByUserIdAsync(userId);

            if (seller == null)
                throw new Exception("Seller not found");


            if (auction.WinnerUserId != userId)
                throw new Exception("You are not allowed to open dispute for this auction");

            var evidenceBytes = new List<byte[]>();

            if (dto.Evidences != null && dto.Evidences.Count > 0)
            {
                foreach (var base64 in dto.Evidences)
                {
                    try
                    {
                        var bytes = Convert.FromBase64String(base64);
                        evidenceBytes.Add(bytes);
                    }
                    catch
                    {
                        throw new Exception("Invalid Base64 string in evidences");
                    }
                }
            }

            var dispute = new Disputes
            {
                Title = $"Dispute for Auction #{auction.Id}",
                Description = dto.Description,
                Reason = dto.Description,
                ResolutionType = dto.ResolutionType,
                Evidences = evidenceBytes,
                Status = DisputeStatus.Open,
                Date = DateTime.UtcNow,
                UserId = userId,
                AuctionId = auction.Id,
                DeliveryId = delivery.Id
            };

            _disputeRepo.Add(dispute);
            var conversation = _chatService.GetOrCreateConversation(dispute.Id, userId);

            await _notificationService.SendDisputeCreatedNotification(
            dispute.Id,
            seller.UserId,
            dispute.Title,
            dispute.Description,
            conversation.Id
        );

            return conversation;
        }

        public async Task<(AuthResult, PagedResult<DisputeDto>)>
                    GetUserReports(string userId, int page = 1, int pageSize = 10)
        {
            var query = _disputeRepo.GetUserDisputes(userId);
            var totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return (
                    new AuthResult
                    {
                        IsSuccess = false,
                        Message = "No reports found"
                    },
                    new PagedResult<DisputeDto>
                    {
                        Data = new List<DisputeDto>(),
                        CurrentPage = page,
                        TotalPages = 0,
                        TotalCount = 0,
                        HasNextPage = false
                    }
                );
            }
            var pagedDisputes = await query
                .OrderByDescending(d => d.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DisputeDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    Status = d.Status,
                    ProblemType = d.ProblemType,
                    Description = d.Description,
                    ResolutionType = d.ResolutionType,
                    Evidences = d.Evidences,
                    Reason = d.Reason,
                    Date = d.Date,
                    AuctionId = d.AuctionId,
                    AuctionTitle = d.Auction.Title
                })
                .ToListAsync();

            return (
                new AuthResult
                {
                    IsSuccess = true,
                    Message = "Success"
                },
                new PagedResult<DisputeDto>
                {
                    Data = pagedDisputes,
                    CurrentPage = page,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasNextPage = page * pageSize < totalCount
                }
            );
        }

        public async Task<DisputeTrackingDto> GetDisputeTracking(int disputeId)
        {
            var dispute = _disputeRepo.GetAll()
                .FirstOrDefault(d => d.Id == disputeId);

            if (dispute == null)
                throw new Exception("Dispute not found");

            string statusText;

            if (dispute.Status == DisputeStatus.Open)
            {
                statusText = "Waiting for seller";
            }
            else if (dispute.Status == DisputeStatus.Negotiation)
            {
                statusText = "Negotiation";
            }
            else if (dispute.Status == DisputeStatus.UnderReview)
            {
                statusText = "Admin Review";
            }
            else if (dispute.Status == DisputeStatus.Resolved)
            {
                statusText = "Resolved";
            }
            else
            {
                statusText = "Rejected";
            }

            var deadline = dispute.Date.AddDays(3);
            var remaining = deadline - DateTime.UtcNow;

            int days = 0;
            int hours = 0;
            int minutes = 0;

            if (remaining > TimeSpan.Zero)
            {
                days = remaining.Days;
                hours = remaining.Hours;
                minutes = remaining.Minutes;
            }

            bool isExpired = remaining <= TimeSpan.Zero;

            bool canChat = !isExpired;
            bool canEscalate = isExpired;
            bool canCancel = true;

            return new DisputeTrackingDto
            {
                DisputeId = dispute.Id,
                Status = statusText,

                Days = days,
                Hours = hours,
                Minutes = minutes,

                CanChat = canChat,
                CanEscalate = canEscalate,
                CanCancel = canCancel
            };
        }

        public async Task CancelDisputeAsync(int disputeId, string userId)
        {
            var dispute = _disputeRepo.GetById(disputeId);

            if (dispute == null)
                throw new Exception("Dispute not found");

            if (dispute.UserId != userId)
                throw new Exception("You are not allowed to cancel this dispute");

            if (dispute.Status == SAFQA.DAL.Enums.DisputeStatus.Resolved)
                throw new Exception("Cannot cancel a Resolved dispute");


            dispute.IsDeleted = true;

            _disputeRepo.Update(dispute);
        }

        public DisputeAdmDto GetDisputeDetails(int disputeId)
        {
            return _disputeRepo.GetAll()

                .Where(d =>
                    d.Id == disputeId &&
                    !d.IsDeleted)

                .Select(d => new DisputeAdmDto
                {
                    Description = d.Description,
                    Reason = d.Reason,
                    Evidences = d.Evidences
                })
                .FirstOrDefault();
        }
    }
}