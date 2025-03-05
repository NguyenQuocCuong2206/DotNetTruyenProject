using CloudinaryDotNet.Actions;

namespace DotNetTruyen.Services
{
    public interface IPhoToService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<List<ImageUploadResult>> AddListPhotoAsync(List<IFormFile> files);
    }
}
