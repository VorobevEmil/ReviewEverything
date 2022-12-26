namespace ReviewEverything.Shared.Contracts.Responses
{
    public class UserManagementResponse
    {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public bool Admin { get; set; }
    }
}
