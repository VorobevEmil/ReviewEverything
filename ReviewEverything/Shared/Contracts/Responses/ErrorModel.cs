namespace ReviewEverything.Shared.Contracts.Responses
{
    public class ErrorModel
    {
        public string FieldName { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
