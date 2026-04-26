using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.NotificationService
{
    public interface INotificationService
    {
        Task SendAuctionStatusUpdated(int auctionId, string status, List<string> userIds);
        Task SendAuctionNotification(int auctionId, decimal price, string type);
        Task SendAuctionFinishedNotification(int id, decimal finalPrice, string? winnerUserId, List<string> userIds);
    }
}
