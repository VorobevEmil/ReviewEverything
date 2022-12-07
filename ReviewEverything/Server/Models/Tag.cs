namespace ReviewEverything.Server.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public List<Review> Reviews { get; set; } = default!;
    }
}
