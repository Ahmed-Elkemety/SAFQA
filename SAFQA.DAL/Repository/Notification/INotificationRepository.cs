using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Notification
{
    public interface INotificationRepository
    {
        Task<List<Models.Notification>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(List<Models.Notification> notifications);
        Task DeleteNotificationsAsync(List<int> ids, string userId);
        Task SaveChangesAsync();
    }
}
