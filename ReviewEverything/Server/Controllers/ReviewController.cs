using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<List<ReviewResponse>>> GetAll(int page, int pageSize, SortReviewByProperty sortByProperty, int? filterByAuthorScore, int? filterByCompositionId, int? categoryId, string? userId, string? idTags, CancellationToken token)
        {
            try
            {
                List<int> tags = null!;
                if (idTags is { Length: > 1 })
                {
                    tags = idTags.Split('.').Select(int.Parse).ToList();
                }
                else if (idTags is { Length: 1 })
                {
                    tags = new List<int>() { int.Parse(idTags) };
                }

                var reviews = await _service.GetReviewsAsync(page, pageSize, sortByProperty, filterByAuthorScore, filterByCompositionId, categoryId, userId, tags, token);
                return Ok(_mapper.Map<List<ReviewResponse>>(reviews));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время получения обзоров произошла внутренняя ошибка сервера");
            }
        }

        [HttpGet("search/{search}")]
        public async Task<ActionResult<List<ReviewResponse>>> Search(string search)
        {
            try
            {
                var reviews = await _service.SearchReviewsAsync(search);
                return Ok(_mapper.Map<List<ReviewSearchResponse>>(reviews));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время поиска обзоров произошла внутренняя ошибка сервера");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleReviewResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var review = await _service.GetReviewByIdAsync(id);
                if (review == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<ArticleReviewResponse>(review));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время получения обзора по id произошла внутренняя ошибка сервера");
            }

        }

        [HttpGet("GetSimilarArticles/{reviewId}")]
        public async Task<ActionResult<List<SimilarArticleReviewResponse>>> GetSimilarArticles(int reviewId)
        {
            try
            {
                var articles = await _service.GetSimilarArticleAsync(reviewId);
                if (articles == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<List<SimilarArticleReviewResponse>>(articles));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время получения похожих обзоров произошла внутренняя ошибка сервера");
            }
        }

        [Authorize("ChangingArticle")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> GetEditById([FromRoute] int id)
        {
            try
            {
                var review = await _service.GetReviewByIdAsync(id);
                if (review == null)
                {
                    return NotFound("Обзор для обновления не найден");
                }

                return Ok(_mapper.Map<ReviewRequest>(review));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время получения обзора для редакирования произошла внутренняя ошибка сервера");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewRequest request)
        {
            try
            {
                var review = _mapper.Map<Review>(request);
                review.AuthorId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                review.CreationDate = DateTime.UtcNow;

                var result = await _service.CreateReviewAsync(review);
                return Ok("Обзор успешно создан");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время создания обзора произошла внутренняя ошибка сервера");
            }
        }

        [Authorize("ChangingArticle")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ReviewRequest request)
        {
            try
            {
                var review = _mapper.Map<Review>(request);
                review.Id = id;

                var updated = await _service.UpdateReviewAsync(review);
                if (updated)
                    return Ok("Обзор успешно обновлен");

                return NotFound("Обзор для обновления не найден");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время обновления обзора произошла внутренняя ошибка сервера");
            }
        }

        [Authorize("ChangingArticle")]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> Delete([FromRoute] int reviewId)
        {
            try
            {
                var deleted = await _service.DeleteReviewAsync(reviewId);
                if (deleted)
                    return NoContent();

                return NotFound("Обзор для удаления не найден");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время удаления обзора произошла внутренняя ошибка сервера");
            }
        }
    }
}