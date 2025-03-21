using Microsoft.AspNetCore.SignalR;

namespace DotNetTruyen.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string title, string message, string type, string icon, string link)
        {
            await Clients.All.SendAsync("ReceiveNotification", new
            {
                title,
                message,
                type,
                icon,
                link
            });
        }

        public async Task UpdateUnreadCount(int unreadCount)
        {
            await Clients.All.SendAsync("UpdateUnreadCount", unreadCount);
        }

        public async Task MarkNotificationAsRead(string notificationId)
        {
            await Clients.All.SendAsync("MarkNotificationAsRead", notificationId);
        }
    }
}
