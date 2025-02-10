using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Forum.UI.Helpers
{
    public class MediaHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        //private readonly string[] _allowedVideoExtensions = { ".mp4" };
        private const long _maxImageSize = 10 * 1024 * 1024;
        //private const long _maxVideoSize = 50 * 1024 * 1024;

        public MediaHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string? SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedImageExtensions.Contains(extension)) throw new ArgumentException("ImageFile", "Only JPG, JPEG, PNG and GIF files are supported.");
            if (file.Length > _maxImageSize) throw new ArgumentException("File size can be at most 10 MB.");

            string fileName = Guid.NewGuid().ToString() + extension;
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            string filePath = Path.Combine(uploadPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return "/uploads/" + fileName;
        }

        //public string? SaveVideo(IFormFile file)
        //{
        //    if (file == null || file.Length == 0) return null;

        //    string extension = Path.GetExtension(file.FileName).ToLower();

        //    if (!_allowedVideoExtensions.Contains(extension)) throw new ArgumentException("VideoFile", "Only MP4 files are supported.");
        //    if (file.Length > _maxVideoSize) throw new ArgumentException("File size can be at most 10 MB.");

        //    string fileName = Guid.NewGuid().ToString() + extension;
        //    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "videos");

        //    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        //    string filePath = Path.Combine(uploadPath, fileName);

        //    using (var fileStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        file.CopyTo(fileStream);
        //    }

        //    return "/videos/" + fileName;
        //}
    }
}
