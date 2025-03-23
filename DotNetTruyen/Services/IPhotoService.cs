﻿using CloudinaryDotNet.Actions;

namespace DotNetTruyen.Services
{
    public interface IPhoToService
    {
        Task<string> AddPhotoAsync(IFormFile file);
        Task<List<string>> AddListPhotoAsync(List<IFormFile> files);
        Task DeletePhotoAsync(string imageUrl);
    }
}
