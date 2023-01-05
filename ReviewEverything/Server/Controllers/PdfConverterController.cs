using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfConverterController : ControllerBase
    {
        [HttpGet("{articleId}")]
        public async Task<ActionResult<FileData>> ConvertArticleToPdf(int articleId)
        {
            try
            {
                using var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();
                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    IgnoredDefaultArgs = new[] { "--disable-extensions" }
                });
                await using var page = await browser.NewPageAsync();
                await page.EmulateMediaTypeAsync(MediaType.Screen);
                await page.GoToAsync($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/article/{articleId}");
                await page.WaitForSelectorAsync("#Need_Authorize", new WaitForSelectorOptions() { Timeout = 300000 });
                await page.EvaluateExpressionAsync(
                    "var article = document.getElementById('Article');" +
                    "var elem = document.getElementsByClassName('mud-layout')[0];" +
                    "elem.innerHTML = '';" +
                    "elem.appendChild(article);" +
                    "document.getElementById('Need_Authorize').innerHTML = '';" +
                    "document.getElementById('Convert').innerHTML = '';");
                await Task.Delay(400);
                var pdfContent = await page.PdfDataAsync();

                return new FileData()
                {
                    Data = pdfContent,
                    ContentType = "application/pdf",
                    FileName = $"Article_{articleId}.pdf"
                };
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message} \n {HttpContext.Request.Scheme}://{HttpContext.Request.Host}/article/{articleId}");
            }
        }
    }
}