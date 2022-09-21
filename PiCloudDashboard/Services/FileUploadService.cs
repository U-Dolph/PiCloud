namespace PiCloudDashboard.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<bool> UploadFile(IFormFile file)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "..", "files", file.FileName);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await file.CopyToAsync(fileStream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
