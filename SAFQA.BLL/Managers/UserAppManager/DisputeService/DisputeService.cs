using Microsoft.AspNetCore.SignalR;
using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.UserAppManager.ChatService;
using SAFQA.BLL.Managers.UserAppManager.ConversationService;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Dispute;
using SAFQA.DAL.Repository.Seller;
using SAFQA.DAL.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.DisputeService
{
    public class DisputeService : IDisputeService
    {
        private readonly IDiputeRepo _disputeRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IUserRepo _userRepo;
        private readonly IsellerRepo _sellerRepo;
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hub;
        public DisputeService(
            IDiputeRepo disputeRepo,
            IAuctionRepository auctionRepo,
            IUserRepo userRepo,
            IsellerRepo sellerRepo,
            IChatService chatService,
            IHubContext<ChatHub> hub)
        {
            _disputeRepo = disputeRepo;
            _auctionRepo = auctionRepo;
            _userRepo = userRepo;
            _sellerRepo = sellerRepo;
            _chatService = chatService;
            _hub = hub;
        }

        public async Task<ConversationDto> CreateDispute(string userId, CreateDisputeDto dto)
        {

            var auction = _auctionRepo.GetById(dto.AuctionId);
            if (auction == null)
                throw new Exception("Auction not found");

            var user = _userRepo.GetById(userId);
            if (user == null)
                throw new Exception("User not found");

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
                Status = DisputeStatus.UnderReview,
                Date = DateTime.UtcNow,
                UserId = userId,
                AuctionId = auction.Id
            };

            _disputeRepo.Add(dispute);

            var conversation = _chatService.GetOrCreateConversation(dispute.Id, userId);

            await _hub.Clients
                .User(auction.Seller.UserId)
                .SendAsync("DisputeOpened", new
                {
                    disputeId = dispute.Id,
                    conversationId = conversation.Id,
                    auctionId = auction.Id
                });

            return conversation;
        }

        public async Task<(AuthResult, List<DisputeDto>)> GetUserReports(string userId)
        {
            var disputes = await _disputeRepo.GetUserDisputesAsync(userId);

            if (disputes == null || !disputes.Any())
            {
                return (new AuthResult
                {
                    IsSuccess = false,
                    Message = "No reports found"
                }, new List<DisputeDto>());
            }

            var result = disputes.Select(d => new DisputeDto
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
            }).ToList();

            return (new AuthResult
            {
                IsSuccess = true,
                Message = "Success"
            }, result);
        }

    }
}
