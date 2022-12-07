using Microsoft.AspNetCore.Identity;

namespace ReviewEverything.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<UserScore> UserScores { get; set; } = default!;
        public List<Review> AuthorReviews { get; set; } = default!;
        public List<Comment> Comments { get; set; } = default!;
        public List<Comment> LikeComments { get; set; } = default!;
    }
}
