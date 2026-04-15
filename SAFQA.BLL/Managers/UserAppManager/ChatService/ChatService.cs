using Microsoft.AspNetCore.SignalR;
using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Managers.UserAppManager.ChatService;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Conversation;
using SAFQA.DAL.Repository.Dispute;
using SAFQA.DAL.Repository.Message;
using SAFQA.DAL.Repository.Seller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.ConversationService
{
    public class ChatService : IChatService
    {
        private readonly IConversationRepo _conversationRepo;
        private readonly IDiputeRepo _disputeRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IMessageRepo _messageRepo;
        private readonly IsellerRepo _sellerRepo;
        public ChatService(
            IConversationRepo conversationRepo,
            IDiputeRepo disputeRepo,
            IAuctionRepository auctionRepo,
            IMessageRepo messageRepo,
            IsellerRepo sellerRepo,
            IHubContext<ChatHub> hub)
        {
            _conversationRepo = conversationRepo;
            _disputeRepo = disputeRepo;
            _auctionRepo = auctionRepo;
            _hub = hub;
            _messageRepo = messageRepo;
            _sellerRepo = sellerRepo;
        }

        public ConversationDto GetOrCreateConversation(int disputeId, string userId)
        {
            var dispute = _disputeRepo.GetById(disputeId);
            if (dispute == null)
                throw new Exception("Dispute not found");

            var auction = _auctionRepo.GetById(dispute.AuctionId.Value);
            if (auction == null)
                throw new Exception("Auction not found");

            var seller = _sellerRepo.GetAll()
                .FirstOrDefault(s => s.Id == auction.SellerId);

            if (seller == null)
                throw new Exception("Seller not found");

            var buyerId = dispute.UserId;
            var sellerId = seller.UserId;

            if (userId != buyerId && userId != sellerId)
                throw new Exception("Unauthorized access");

            var existing = _conversationRepo.GetAll()
                .FirstOrDefault(c => c.DisputeId == disputeId);

            if (existing != null)
                return Map(existing);

            var conversation = new Conversation
            {
                BuyerId = buyerId,
                SellerUserId = sellerId,
                DisputeId = dispute.Id,
                Type = ConversationType.Dispute,
                CreatedAt = DateTime.UtcNow
            };

            _conversationRepo.Add(conversation);

            return Map(conversation);
        }

        private ConversationDto Map(Conversation c)
        {
            return new ConversationDto
            {
                Id = c.Id,
                BuyerId = c.BuyerId,
                SellerId = c.SellerUserId,
                DisputeId = c.DisputeId,
                CreatedAt = c.CreatedAt,
                LastMessage = c.LastMessage
            };
        }

        public async Task SendMessage(SendMessageDto dto)
        {
            var conversation = _conversationRepo.GetById(dto.ConversationId);

            if (conversation == null)
                throw new Exception("Conversation not found");

            var message = new Message
            {
                ConversationId = dto.ConversationId,
                SenderId = dto.SenderId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                IsSeen = false
            };

            _messageRepo.Add(message);


            conversation.LastMessage = dto.Content;
            conversation.LastMessageAt = DateTime.UtcNow;
            _conversationRepo.Update(conversation);


            await _hub.Clients
                .Group(dto.ConversationId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    conversationId = dto.ConversationId,
                    senderId = dto.SenderId,
                    content = dto.Content,
                    createdAt = message.CreatedAt
                });
        }
    }
}
