using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Services.CloudImageService
{
    public class CloudImageService : ICloudImageService
    {
        private readonly Cloudinary _cloudinary;
        public CloudImageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:Cloud"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> SendImageOnCloudAsync(FileData fileData, CancellationToken token)
        {
            Stream stream = new MemoryStream(fileData.Data);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileData.FileName, stream),
                PublicId = Guid.NewGuid().ToString(),
                Transformation = new Transformation().Height(300)
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken: token);

            return uploadResult.Url.AbsoluteUri;
        }
    }
}
