using System.Net;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using ReviewEverything.Server.Common.Exceptions;
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
            CheckImage(fileData);

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

        private void CheckImage(FileData fileData)
        {
            if (!fileData.ContentType.Contains("image"))
                throw new HttpStatusRequestException(HttpStatusCode.BadRequest,
                    $"Загруженный файл \"{fileData.FileName}\" не является изображением");

            if (fileData.Data.Length > GetMaxAllowedSize())
                throw new HttpStatusRequestException(HttpStatusCode.BadRequest,
                    $"У изображения \"{fileData.FileName}\" превышен максимальный размер. Максимальный размер файла составляет {GetMaxAllowedSize() / 1024 / 1024} МБ");
        }

        public int GetMaxAllowedSize()
        {
            var maxAllowedSize = 1024 * 1024 * 10;
            return maxAllowedSize;
        }
    }
}
