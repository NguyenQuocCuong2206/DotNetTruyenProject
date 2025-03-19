namespace DotNetTruyen.Services
{
    public interface INotificationService
    {
        Task<int> GetUnreadNotificationCountAsync();
    }
}
