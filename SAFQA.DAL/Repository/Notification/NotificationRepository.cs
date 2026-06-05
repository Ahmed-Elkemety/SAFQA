using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using SAFQA.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SAFQA.DAL.Repository.Notification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SAFQA_Context _context;

        public NotificationRepository(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task<List<Models.Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(List<Models.Notification> notifications)
        {
            foreach (var n in notifications)
            {
                n.IsRead = true;
            }
        }

        public async Task DeleteNotificationsAsync(List<int> ids, string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => ids.Contains(n.Id) && n.UserId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(Models.Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Models.Notification> notifications)
        {
            if (notifications == null || !notifications.Any())
                return;

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }
    }
}
