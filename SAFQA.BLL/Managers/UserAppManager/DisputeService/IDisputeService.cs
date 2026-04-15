using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.DisputeService
{
    public interface IDisputeService
    {
        Task<ConversationDto> CreateDispute(string userId, CreateDisputeDto dto);
    }
}
