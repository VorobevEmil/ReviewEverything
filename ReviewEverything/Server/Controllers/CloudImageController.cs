using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Services.CloudImageService;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudImageController : ControllerBase
    {
        private readonly ICloudImageService _service;
        private readonly IStringLocalizer<CloudImageController> _localizer;

        public CloudImageController(ICloudImageService service, IStringLocalizer<CloudImageController> localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        [HttpPost]
        [RequestSizeLimit(15_000_000)]
        public async Task<ActionResult<string>> SendImageOnCloud(FileData fileData, CancellationToken token)
        {
            try
            {
                return Ok(await _service.SendImageOnCloudAsync(fileData, token));
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest($"{e.Message}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{_localizer["Во время загрузки изображения"].Value} \"{fileData.FileName}\" {_localizer["произошла внутренняя ошибка сервера"].Value}");
            }
        }

        [HttpGet("GetMaxAllowedSize")]
        public int GetMaxAllowedSize()
        {
            return _service.GetMaxAllowedSize();
        }
    }
}