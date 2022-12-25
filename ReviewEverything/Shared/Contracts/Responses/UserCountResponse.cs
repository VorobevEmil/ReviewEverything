namespace ReviewEverything.Shared.Contracts.Responses
{
    public class UserCountResponse
    {
        public UserCountResponse(int count, List<UserResponse> users)
        {
            Count = count;
            Users = users;
        }

        public int Count { get; set; }
        public List<UserResponse> Users { get; set; }
    }
}
