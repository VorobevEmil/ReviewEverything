using Microsoft.AspNetCore.Authorization;
using ReviewEverything.Server.Services.ReviewService;
using System.Security.Claims;

namespace ReviewEverything.Server.Common.Policies
{
    public class PossibilityChangingArticleHandler : AuthorizationHandler<PossibilityChangingArticleHandler>, IAuthorizationRequirement
    {
        private readonly IReviewService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PossibilityChangingArticleHandler(IReviewService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PossibilityChangingArticleHandler requirement)
        {
            try
            {
                if (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    int reviewId = int.Parse(_httpContextAccessor.HttpContext!.GetRouteValue("id")!.ToString()!);

                    var userId = (await _service.GetReviewByIdAsync(reviewId))!.AuthorId;

                    if (userId == context.User.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
            catch
            {
                context.Fail();
            }
        }
    }
}
