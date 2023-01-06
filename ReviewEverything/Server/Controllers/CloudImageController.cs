using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Services.CloudImageService;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudImageController : ControllerBase
    {
        private readonly ICloudImageService _service;

        public CloudImageController(ICloudImageService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<string>> SendImageOnCloud(FileData fileData, CancellationToken token)
        {
            try
            {
                if (!fileData.ContentType.Contains("image"))
                    return BadRequest($"Загруженный файл {fileData.FileName} не является изображением");

                if (fileData.Data.Length > GetMaxAllowedSize())
                    return BadRequest(
                        $"У изображения {fileData.FileName} превышен максимальный размер. Максимальный размер файла составляет {GetMaxAllowedSize() / 1024 / 1024} МБ");

                return Ok(await _service.SendImageOnCloudAsync(fileData, token));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Во время загрузки изображения {fileData.FileName} произошла внутренняя ошибка сервера");
            }
        }

        [HttpGet("GetMaxAllowedSize")]
        public int GetMaxAllowedSize()
        {
            //max allowed size 10 mb
            var maxAllowedSize = 1024 * 1024 * 10;
            return maxAllowedSize;
        }
    }
}