namespace ReviewEverything.Shared.Contracts.Responses
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public string Body { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string User { get; set; } = default!;
        public DateTime CreationDate { get; set; }
    }
}
