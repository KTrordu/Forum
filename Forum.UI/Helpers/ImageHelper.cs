using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Forum.UI.Helpers
{
    public static class ImageHelper
    {
        private static string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        public static string SaveImage(IFormFile imageFile)
        {
            if (imageFile == null) return null;

            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            string extension = Path.GetExtension(imageFile.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(_uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

        public static void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            string filePath = Path.Combine(_uploadsFolder, Path.GetFileName(imagePath));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
