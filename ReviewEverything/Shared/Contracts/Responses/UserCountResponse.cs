namespace ReviewEverything.Shared.Contracts.Responses
{
    public class UserCountResponse
    {
        public UserCountResponse(int count, List<UserManagementResponse> users)
        {
            Count = count;
            Users = users;
        }

        public int Count { get; set; }
        public List<UserManagementResponse> Users { get; set; }
    }
}
