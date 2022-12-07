namespace ReviewEverything.Server.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public List<Composition> Compositions { get; set; } = default!;
    }
}
