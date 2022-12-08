
namespace ReviewEverything.Shared.Contracts.Responses
{
    public class UserResponse
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public List<ReviewResponse> Reviews { get; set; } = default!;
    }
}