using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IEnumerable<string> GetAllFromImagesDirectory(string imagesDirectory)
        {
            var images = new List<string>();
            try
            {
                var fullyImagesDirectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, Constants.ImagesPath, imagesDirectory);
                images = Directory.GetFiles(Path.Combine(_webHostEnvironment.WebRootPath, fullyImagesDirectoryPath), "*.*").Select(file => Path.Combine("\\", Constants.ImagesPath, imagesDirectory, Path.GetFileName(file))).ToList();
                images = images.Select(_ => _.Replace("\\", "/")).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return images;
        }

        public async Task SaveImagesToDirectoryAsync(string directoryPath, List<IFormFile> images)
        {
            for (int i = 0; i < images.Count(); i++)
            {
                var imageFilePath = Path.Combine(directoryPath, images[i].FileName);
                using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await images[i].CopyToAsync(fileStream);
                }
            }
        }

        public void DeleteImagesFromDirectoryAsync(string directoryPath, List<string> images)
        {
            if (Directory.Exists(directoryPath))
            {
                foreach (var image in images)
                {
                    var imageFilePath = Path.Combine(directoryPath, image);
                    if (File.Exists(imageFilePath))
                    {
                        File.Delete(imageFilePath);
                    }
                }
            }
        }

    }
}
