namespace ReviewEverything.Shared.Contracts.Responses
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int CompositionId { get; set; }
        public CompositionResponse Composition { get; set; } = default!;
    }
}