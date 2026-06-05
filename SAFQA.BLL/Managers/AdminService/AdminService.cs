using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.AnnouncementDto;
using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using SAFQA.BLL.Dtos.UserAppDto.paymentsAdmDto;
using SAFQA.BLL.Managers.BackgroundServices;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Conversation;
using SAFQA.DAL.Repository.Dispute;
using SAFQA.DAL.Repository.Notification;
using SAFQA.DAL.Repository.Transaction;
using SAFQA.DAL.Repository.Users;
using SAFQA.DAL.Repository.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IDiputeRepo _disputeRepo;
        private readonly IConversationRepo _conversationRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IWalletRepo _walletRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserRepo _userRepo;
        IHubContext<NotificationHub> _hubContext;

        public AdminService(IDiputeRepo diputeRepo, IConversationRepo conversationRepo, ITransactionRepository transactionRepo, IAuctionRepository auctionRepo, IWalletRepo walletRepo, INotificationRepository notificationRepo, IUserRepo userRepo, IHubContext<NotificationHub> hubContext)
        {
            _disputeRepo = diputeRepo;
            _conversationRepo = conversationRepo;
            _transactionRepo = transactionRepo;
            _auctionRepo = auctionRepo;
            _userRepo = userRepo;
            _notificationRepo = notificationRepo;
            _walletRepo = walletRepo;
            _hubContext = hubContext;
        }

        public List<EscalateCardDTO> GetEscalatedCards()
        {
            var result = _disputeRepo.GetAll()
                .Where(d => !d.IsDeleted &&
                            d.Status == DisputeStatus.UnderReview)
                .Select(d => new EscalateCardDTO
                {
                    DisputeId = d.Id,

                    AuctionTitle = d.Auction.Title,

                    SellerName = d.Auction.Seller.User.FullName,

                    BuyerName = d.User.FullName,

                    PaidAmount =
                        d.Auction.FinalPrice,

                    Evidences = d.Evidences
                })
                .ToList();

            return result;
        }

        public ConversationDto GetDisputeChat(int disputeId)
        {
            var conversation = _conversationRepo.Get(disputeId);

            if (conversation == null)
                return null;

            return new ConversationDto
            {
                Id = conversation.Id,
                DisputeId = conversation.DisputeId,

                BuyerId = conversation.Buyer.FullName,
                SellerId = conversation.SellerUser.FullName,

                CreatedAt = conversation.CreatedAt,

                Messages = conversation.Messages
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new MessageDto
                    {
                        MessageId = m.Id,
                        SenderName = m.Sender.FullName,
                        Content = m.Content,
                        CreatedAt = m.CreatedAt,
                        IsSeen = m.IsSeen,

                        Attachments = m.Attachments != null
                            ? m.Attachments.Select(a => a.FileUrl).ToList()
                            : new List<string>()
                    })
                    .ToList()
            };
        }

        public List<SuccessfulPaymentDto> GetSuccessfulPayments(int days)
        {
            var fromDate = DateTime.Now.AddDays(-days);

            var result = _transactionRepo.GetAll()
                .Where(t =>
                    t.Status == TransactionStatus.Completed &&
                    t.CreatedAt >= fromDate)
                .Select(t => new SuccessfulPaymentDto
                {
                    UserName = t.Wallet.User.FullName,
                    Amount = t.Amount,

                    Method = t.Wallet.SavedCards
                        .OrderByDescending(c => c.CreatedAt)
                        .Select(c => c.CardBrand)
                        .FirstOrDefault() ?? "Visa",

                    Date = t.CreatedAt
                })
                .OrderByDescending(x => x.Date)
                .ToList();

            return result;
        }

        public List<FailedPaymentDto> GetFailedPayments(int days)
        {
            var fromDate = DateTime.Now.AddDays(-days);

            var transactions = _transactionRepo.GetAll()
                .Where(t =>
                    t.Status == TransactionStatus.Failed &&
                    t.CreatedAt >= fromDate)
                .Select(t => new
                {
                    t.Wallet.User.FullName,
                    t.Amount,
                    t.CreatedAt,
                    t.Description,
                    Method = t.Wallet.SavedCards
                        .OrderByDescending(c => c.CreatedAt)
                        .Select(c => c.CardBrand)
                        .FirstOrDefault()
                })
                    .ToList();

            return transactions.Select(t => new FailedPaymentDto
            {
                UserName = t.FullName,
                Amount = t.Amount,
                Date = t.CreatedAt,
                Reason = MapFailureReason(t.Description),
                Method = t.Method ?? "Unknown"
            }).ToList();
        }
        private string MapFailureReason(string rawMessage)
        {
            if (string.IsNullOrEmpty(rawMessage))
                return "Unknown error occurred";

            rawMessage = rawMessage.ToLower();

            if (rawMessage.Contains("insufficient") || rawMessage.Contains("fund"))
                return "Insufficient funds in the account";

            if (rawMessage.Contains("network") || rawMessage.Contains("timeout"))
                return "Network issue, please try again";

            if (rawMessage.Contains("declined") || rawMessage.Contains("do not honor"))
                return "Payment was declined by the bank";

            if (rawMessage.Contains("expired"))
                return "Card has expired";

            if (rawMessage.Contains("invalid card"))
                return "Invalid card details";

            return "Payment failed due to an unknown reason";
        }


        public RefundDto FullRefund(int disputeId)
        {
            var dispute = _disputeRepo.GetAll()
                .FirstOrDefault(d => d.Id == disputeId && !d.IsDeleted);

            if (dispute == null)
                throw new Exception("Dispute not found.");



            var auction = _auctionRepo.GetAll()
                .FirstOrDefault(a => a.Id == dispute.AuctionId);

            if (auction == null)
                throw new Exception("Auction not found.");



            if (string.IsNullOrEmpty(auction.WinnerUserId))
                throw new Exception("No winner found.");



            var wallet = _walletRepo.GetAll()
                .FirstOrDefault(w => w.UserId == auction.WinnerUserId);

            if (wallet == null)
                throw new Exception("Wallet not found.");


            bool alreadyRefunded =
                _transactionRepo.GetAll()
                .Any(t =>
                    t.Wallet.UserId == auction.WinnerUserId &&
                    t.Type == TransactionType.Transfer &&
                    t.Description.Contains($"Auction #{auction.Id}"));

            if (alreadyRefunded)
                throw new Exception("Refund already processed.");


            var paidAmount = _transactionRepo.GetAll()
                .Where(t =>
                    t.Wallet.UserId == auction.WinnerUserId &&
                    t.Status == TransactionStatus.Completed &&
                    t.Description.Contains($"Auction #{auction.Id}"))
                .Sum(t => t.Amount);


            if (paidAmount <= 0)
                throw new Exception("No paid amount found.");


            decimal balanceBefore = wallet.Balance;

            wallet.Balance += paidAmount;
            wallet.UpdatedAt = DateTime.Now;

            _walletRepo.Update(wallet);



            var refundTransaction = new Transactions
            {
                WalletId = wallet.Id,

                Type = TransactionType.Transfer,

                Status = TransactionStatus.Completed,

                Amount = paidAmount,

                BalanceBefore = balanceBefore,

                BalanceAfter = wallet.Balance,

                Description = $"Full refund for Auction #{auction.Id}",

                CreatedAt = DateTime.Now
            };

            _transactionRepo.Add(refundTransaction);


            dispute.Status = DisputeStatus.Resolved;
            dispute.ResolutionType = DisputeResolutionType.FullRefund;

            _disputeRepo.Update(dispute);



            return new RefundDto
            {
                Success = true,
                Message = "Full refund completed successfully.",
                RefundedAmount = paidAmount
            };
        }

        public RefundDto PartialRefund(int disputeId, decimal refundAmount)
        {
            var dispute = _disputeRepo.GetAll()
                .FirstOrDefault(d => d.Id == disputeId && !d.IsDeleted);

            if (dispute == null)
                throw new Exception("Dispute not found.");



            var auction = _auctionRepo.GetAll()
                .FirstOrDefault(a => a.Id == dispute.AuctionId);

            if (auction == null)
                throw new Exception("Auction not found.");



            if (string.IsNullOrEmpty(auction.WinnerUserId))
                throw new Exception("No winner found.");



            var wallet = _walletRepo.GetAll()
                .FirstOrDefault(w => w.UserId == auction.WinnerUserId);

            if (wallet == null)
                throw new Exception("Wallet not found.");


            bool alreadyRefunded =
                _transactionRepo.GetAll()
                .Any(t =>
                    t.Wallet.UserId == auction.WinnerUserId &&
                    t.Type == TransactionType.Transfer &&
                    t.Description.Contains($"Auction #{auction.Id}"));

            if (alreadyRefunded)
                throw new Exception("Refund already processed.");



            if (refundAmount <= 0)
                throw new Exception("Invalid refund amount.");



            if (refundAmount > auction.FinalPrice)
                throw new Exception("Refund cannot exceed total paid amount.");



            decimal balanceBefore = wallet.Balance;

            wallet.Balance += refundAmount;
            wallet.UpdatedAt = DateTime.Now;

            _walletRepo.Update(wallet);



            var transaction = new Transactions
            {
                WalletId = wallet.Id,
                Type = TransactionType.Transfer,
                Status = TransactionStatus.Completed,
                Amount = refundAmount,
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance,
                Description = $"Partial refund for Auction #{auction.Id}",
                CreatedAt = DateTime.Now
            };

            _transactionRepo.Add(transaction);



            dispute.Status = DisputeStatus.Resolved;
            dispute.ResolutionType = DisputeResolutionType.PartialRefund;

            _disputeRepo.Update(dispute);



            return new RefundDto
            {
                Success = true,
                Message = "Partial refund completed successfully.",
                RefundedAmount = refundAmount
            };
        }

        public async Task SendGlobalAnnouncement(SendAnnouncementDto dto)
{
    if (string.IsNullOrWhiteSpace(dto.Title))
        throw new Exception("Title is required");

    if (string.IsNullOrWhiteSpace(dto.Message))
        throw new Exception("Message is required");

    // 1) Get only user IDs (lighter than full entities)
    var userIds = _userRepo.GetAll()
        .Where(u => !u.IsDeleted)
        .Select(u => u.Id)
        .ToList();

    var now = DateTime.UtcNow;

    // 2) Prepare notifications in memory (NO DB CALL YET)
    var notifications = userIds.Select(userId => new Notification
    {
        Title = dto.Title,
        Message = dto.Message,
        UserId = userId,
        IsRead = false,
        CreatedAt = now,
        notificationType = NotificationTypes.Announcement,
        ReferenceId = null
    }).ToList();

    // 3) BULK INSERT (FAST)
    await _notificationRepo.AddRangeAsync(notifications);

    // 4) SignalR broadcast (instant, no waiting for DB)
    await _hubContext.Clients.All.SendAsync(
        "ReceiveAnnouncement",
        new
        {
            Title = dto.Title,
            Message = dto.Message,
            CreatedAt = now
        });
}
    }
}
