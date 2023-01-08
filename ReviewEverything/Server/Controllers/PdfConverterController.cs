using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using ReviewEverything.Server.Services.PdfConvertService;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfConverterController : ControllerBase
    {
        private readonly IPdfConvertService _service;

        public PdfConverterController(IPdfConvertService service)
        {
            _service = service;
        }

        [HttpGet("{articleId}")]
        public async Task<ActionResult<FileData>> ConvertArticleToPdf(int articleId)
        {
            try
            {
                return Ok(await _service.ConvertArticleToPdfAsync(articleId));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}