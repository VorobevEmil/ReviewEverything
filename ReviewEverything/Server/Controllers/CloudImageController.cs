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
                    return BadRequest($"����������� ���� {fileData.FileName} �� �������� ������������");

                if (fileData.Data.Length > GetMaxAllowedSize())
                    return BadRequest(
                        $"� ����������� {fileData.FileName} �������� ������������ ������. ������������ ������ ����� ���������� {GetMaxAllowedSize() / 1024 / 1024} ��");

                return Ok(await _service.SendImageOnCloudAsync(fileData, token));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"�� ����� �������� ����������� {fileData.FileName} ��������� ���������� ������ �������");
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