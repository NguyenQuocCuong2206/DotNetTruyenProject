namespace DotNetTruyen.ViewModels.Management
{
    public class AdvertisementsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string LinkTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string? ImageUrlPath { get; set; }
    }
}
