namespace ReviewEverything.Shared.Contracts.Responses
{
    public class CloudImageResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}
