using SAFQA.BLL.Dtos.AnnouncementDto;
using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using SAFQA.BLL.Dtos.UserAppDto.paymentsAdmDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.AdminService
{
    public interface IAdminService
    {
        List<EscalateCardDTO> GetEscalatedCards();
        ConversationDto GetDisputeChat(int disputeId);
        List<SuccessfulPaymentDto> GetSuccessfulPayments(int days);
        List<FailedPaymentDto> GetFailedPayments(int days);
        RefundDto FullRefund(int disputeId);
        RefundDto PartialRefund(int disputeId, decimal refundAmount);
        Task SendGlobalAnnouncement(SendAnnouncementDto dto);
    }
}
