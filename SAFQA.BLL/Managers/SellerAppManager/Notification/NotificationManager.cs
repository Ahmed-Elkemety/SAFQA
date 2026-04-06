using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.SellerAppDto.NotificationDto;
using SAFQA.DAL.Repository.Notification;

namespace SAFQA.BLL.Managers.SellerAppManager.Notification
{
    public class NotificationManager : INotificationManager
    {
        private readonly INotificationRepository _repo;

        public NotificationManager(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _repo.GetUserNotificationsAsync(userId);


            var result = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.notificationType.ToString(),
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();

            var unread = notifications.Where(n => !n.IsRead).ToList();
            await _repo.MarkAsReadAsync(unread);
            await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<bool> DeleteNotificationsAsync(DeleteNotificationsDto dto, string userId)
        {
            await _repo.DeleteNotificationsAsync(dto.NotificationIds, userId);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
