using Microsoft.AspNetCore.Identity;

namespace ReviewEverything.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        //public string FirstName { get; set; } = default!;
        //public string? MiddleName { get; set; } = default!;
        //public string LastName { get; set; } = default!;
        //public string? AboutMe { get; set; } = default!;

        public List<UserScore> UserScores { get; set; } = default!;
        public List<Review> AuthorReviews { get; set; } = default!;
        public List<Comment> Comments { get; set; } = default!;
        public List<Review> LikeReviews { get; set; } = default!;
    }
}
