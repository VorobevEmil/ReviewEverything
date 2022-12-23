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
            return Ok(await _service.SendImageOnCloudAsync(fileData, token));
        }
    }

}