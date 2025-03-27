namespace DotNetTruyen.Services
{
    public interface INotificationService
    {
        Task<int> GetUnreadNotificationCountAsync(string userId = null, bool isAdmin = false);
    }
}
