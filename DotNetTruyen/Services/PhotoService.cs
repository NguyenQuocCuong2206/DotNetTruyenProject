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

        public async Task<List<ImageUploadResult>> AddListPhotoAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return null;
            }

            var uploadResults = new List<ImageUploadResult>();

            foreach (var file in files)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream)
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    uploadResults.Add(uploadResult);
                }
            }

            return uploadResults;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
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
                return uploadResult;
            }
        }
    }
}
