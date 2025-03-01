using Microsoft.AspNetCore.SignalR;
using DotNetTruyen.ViewModels;

namespace DotNetTruyen.Hubs
{
    public class GenreHub : Hub
    {
        public async Task SendGenreCreated(GenreViewModel genre)
        {
            await Clients.All.SendAsync("ReceiveGenreCreated", genre);
        }

        public async Task SendGenreUpdated(GenreViewModel genre)
        {
            await Clients.All.SendAsync("ReceiveGenreUpdated", genre);
        }

        public async Task SendGenreDeleted(Guid genreId)
        {
            await Clients.All.SendAsync("ReceiveGenreDeleted", genreId);
        }
    }
}
