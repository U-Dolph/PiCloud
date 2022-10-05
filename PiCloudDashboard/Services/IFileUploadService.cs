using PiCloudDashboard.Models;

namespace PiCloudDashboard.Services
{
    public interface IFileUploadService
    {
        Task<Tuple<bool, string>> UploadFile(IFormFile file, Game game);
    }
}
