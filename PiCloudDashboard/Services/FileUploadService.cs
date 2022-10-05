using PiCloudDashboard.Models;

namespace PiCloudDashboard.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<Tuple<bool, string>> UploadFile(IFormFile file, Game game)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "..", "files", $"{game.Name}-{file.FileName}");

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    return Tuple.Create(true, $"{game.Name}-{file.FileName}");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, "None");
            }

        }
    }
}
