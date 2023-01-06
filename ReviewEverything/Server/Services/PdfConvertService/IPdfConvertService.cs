using ReviewEverything.Shared.Models;

namespace ReviewEverything.Server.Services.PdfConvertService
{
    public interface IPdfConvertService
    {
        Task<FileData> ConvertArticleToPdfAsync(int articleId);
    }
}