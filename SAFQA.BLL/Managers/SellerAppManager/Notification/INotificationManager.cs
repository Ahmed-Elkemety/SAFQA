using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.SellerAppDto.NotificationDto;

namespace SAFQA.BLL.Managers.SellerAppManager.Notification
{
    public interface INotificationManager
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);
        Task<bool> DeleteNotificationsAsync(DeleteNotificationsDto dto, string userId);
    }
}
