
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
        public async Task<int> GetUnreadNotificationCountAsync()
        {
            return await _context.Notifications
                .CountAsync(n => n.DeletedAt == null && !n.IsRead);
        }
    }
}
