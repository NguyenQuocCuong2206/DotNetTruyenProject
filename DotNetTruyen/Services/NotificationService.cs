
using DotNetTruyen.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetTruyen.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DotNetTruyenDbContext _context;

        public NotificationService(DotNetTruyenDbContext context)
        {
            _context = context;
        }
        public async Task<int> GetUnreadNotificationCountAsync(string userId = null, bool isAdmin = false)
        {
            if (isAdmin)
            {

                return await _context.Notifications
                    .CountAsync(n => n.DeletedAt == null && !n.IsRead && n.UserId == null);
            }
            else if (!string.IsNullOrEmpty(userId))
            {

                return await _context.Notifications
                    .CountAsync(n => n.DeletedAt == null && !n.IsRead && n.UserId == Guid.Parse(userId));
            }
            else
            {

                return 0;
            }
        }
    }
}
