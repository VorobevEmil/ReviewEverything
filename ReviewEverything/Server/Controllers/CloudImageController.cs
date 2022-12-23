using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PSC.Blazor.Components.MarkdownEditor.Models;
using ReviewEverything.Server.Services.CloudImageService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Models;
using System.Net;
using Microsoft.Net.Http.Headers;
using ReviewEverything.Server.Common.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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