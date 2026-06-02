using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;

namespace SAFQA.BLL.Managers.UserAppManager.DisputeService
{
    public interface IDisputeService
    {
        Task<ConversationDto> CreateDispute(string userId, CreateDisputeDto dto);
        Task<(AuthResult, PagedResult<DisputeDto>)> GetUserReports(string userId, int page = 1, int pageSize = 10);
        Task<DisputeTrackingDto> GetDisputeTracking(int disputeId);

        Task CancelDisputeAsync(int disputeId, string userId);
        DisputeAdmDto GetDisputeDetails(int disputeId);
    }
}