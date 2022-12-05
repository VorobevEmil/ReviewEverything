namespace ReviewEverything.Shared.Models.Account
{
    public class ApiClaim
    {
        public ApiClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; }
        public string Value { get; set; }
    }
}
