namespace PiCloudDashboard.Services
{
    public interface IFileUploadService
    {
        Task<bool> UploadFile(IFormFile file);
    }
}
