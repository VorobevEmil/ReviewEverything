namespace ReviewEverything.Shared.Models.Account
{
    public class UserInfo
    {
        public UserInfo()
        {
            Claims = new List<ApiClaim>();
        }

        public string AuthenticationType { get; set; } = default!;
        public IEnumerable<ApiClaim> Claims { get; set; }
    }
}
