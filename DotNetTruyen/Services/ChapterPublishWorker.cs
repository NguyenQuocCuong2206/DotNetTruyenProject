using DotNetTruyen.Data;
using DotNetTruyen.Hubs;
using DotNetTruyen.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTruyen.Services
{
    public class ChapterPublishWorker : BackgroundService
    {
        private readonly ILogger<ChapterPublishWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ChapterPublishWorker(
            ILogger<ChapterPublishWorker> logger,
            IServiceProvider serviceProvider,
            IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ChapterPublishWorker running at: {time}", DateTimeOffset.Now);

                try
                {

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DotNetTruyenDbContext>();


                        var chaptersToPublish = await dbContext.Chapters
                            .Include(c => c.Comic)
                            .Where(c => !c.IsPublished
                                     && c.DeletedAt == null
                                     && c.PublishedDate.HasValue
                                     && c.PublishedDate <= DateTime.Now)
                            .ToListAsync(stoppingToken);


                        if (!chaptersToPublish.Any())
                        {
                            _logger.LogInformation("No chapters to publish at this time.");
                            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                            continue; 
                        }

                        foreach (var chapter in chaptersToPublish)
                        {
                            chapter.IsPublished = true;
                            chapter.UpdatedAt = DateTime.Now;
                            _logger.LogInformation($"Chapter {chapter.ChapterNumber} of Comic {chapter.ComicId} has been published.");


                            var notification = new Notification
                            {
                                Id = Guid.NewGuid(),
                                Title = "Chapter mới được publish",
                                Message = $"Chapter {chapter.ChapterNumber} của truyện '{chapter.Comic.Title}' đã được đăng.",
                                Type = "success",
                                Icon = "check-circle",
                                Link = $"/Comic/{chapter.ComicId}/Chapter/{chapter.Id}",
                                IsRead = false
                            };

                            dbContext.Notifications.Add(notification);


                            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                            {
                                id = notification.Id,
                                title = notification.Title,
                                message = notification.Message,
                                type = notification.Type,
                                icon = notification.Icon,
                                link = notification.Link
                            }, stoppingToken);
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while publishing chapters.");
                }


                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}