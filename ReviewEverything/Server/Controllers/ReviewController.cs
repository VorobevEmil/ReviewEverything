using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.ReviewService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewResponse>>> GetAll()
        {
            try
            {
                var reviews = await _service.GetReviewsAsync();
                return Ok(_mapper.Map<List<ReviewResponse>>(reviews));
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, TypeReviewResponse typeReview = TypeReviewResponse.Views)
        {
            var review = await _service.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return typeReview switch
            {
                TypeReviewResponse.Edit => Ok(_mapper.Map<ReviewRequest>(review)),
                TypeReviewResponse.Article => Ok(_mapper.Map<ArticleReviewResponse>(review)),
                _ => Ok(_mapper.Map<ReviewResponse>(review))

            };
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewRequest request)
        {
            var review = _mapper.Map<Review>(request);
            review.AuthorId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            var result = await _service.CreateReviewAsync(review);
            return Created(Url.Action($"GetById", new { id = review.Id })!, review);
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> Update([FromRoute] int reviewId, [FromBody] ReviewRequest request)
        {
            var review = _mapper.Map<Review>(request);
            review.Id = reviewId;

            var updated = await _service.UpdateReviewAsync(review);
            if (updated)
                return Ok(review);

            return NotFound();
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> Delete([FromRoute] int reviewId)
        {
            var deleted = await _service.DeleteReviewAsync(reviewId);
            if (deleted)
                return NoContent();

            return NotFound();
        }

    }
}
