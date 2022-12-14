namespace ReviewEverything.Shared.Contracts.Requests
{
    public class CommentRequest
    {
        public string Body { get; set; } = default!;
        public int ReviewId { get; set; } = default!;
    }
}
