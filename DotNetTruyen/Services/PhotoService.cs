using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace DotNetTruyen.Services
{
    public class PhotoService : IPhoToService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<List<string>> AddListPhotoAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return new List<string>();
            }

            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream)
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.SecureUrl.ToString()))
                    {
                        imageUrls.Add(uploadResult.Url.ToString());
                    }
                }
            }

            return imageUrls;
        }

        public async Task<string> AddPhotoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream)
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult?.Url.ToString();
            }
        }
    }
}
