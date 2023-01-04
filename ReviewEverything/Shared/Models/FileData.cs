using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Models
{
    public class FileData
    {
        public byte[] Data { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
    }
}
