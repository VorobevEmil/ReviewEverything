using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Services.CloudImageService
{
    public interface ICloudImageService
    {
        Task<string> SendImageOnCloudAsync(FileData fileData, CancellationToken token);
    }
}
