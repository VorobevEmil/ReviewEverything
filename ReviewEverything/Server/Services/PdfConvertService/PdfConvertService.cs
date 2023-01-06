using PuppeteerSharp;
using PuppeteerSharp.Media;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Services.PdfConvertService
{
    public class PdfConvertService : IPdfConvertService
    {
        private readonly IHttpContextAccessor _accessor;

        public PdfConvertService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public async Task<FileData> ConvertArticleToPdfAsync(int articleId)
        {
            using var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions());
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" },
                IgnoredDefaultArgs = new[] { "--disable-extensions" }
            });
            await using var page = await browser.NewPageAsync();
            await page.EmulateMediaTypeAsync(MediaType.Screen);
            await page.GoToAsync($"{_accessor.HttpContext!.Request.Scheme}://{_accessor.HttpContext!.Request.Host}/article/{articleId}");
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
    }
}