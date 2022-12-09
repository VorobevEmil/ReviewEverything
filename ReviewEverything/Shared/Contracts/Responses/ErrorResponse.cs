namespace ReviewEverything.Shared.Contracts.Responses
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = default!;
    }
}
