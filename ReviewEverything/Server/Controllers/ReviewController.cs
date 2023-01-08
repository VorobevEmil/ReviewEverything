using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
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
        private readonly IStringLocalizer<ReviewController> _localizer;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService service, IStringLocalizer<ReviewController> localizer, IMapper mapper)
        {
            _service = service;
            _localizer = localizer;
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
                return StatusCode(StatusCodes.Status500InternalServerError);
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
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleReviewResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var review = await _service.GetReviewByIdAsync(id);

                return Ok(_mapper.Map<ArticleReviewResponse>(review));
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer[e.Message]);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("GetSimilarArticles/{reviewId}")]
        public async Task<ActionResult<List<SimilarArticleReviewResponse>>> GetSimilarArticles(int reviewId)
        {
            try
            {
                var articles = await _service.GetSimilarArticleAsync(reviewId);
                return Ok(_mapper.Map<List<SimilarArticleReviewResponse>>(articles));
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer[e.Message].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize("ChangingArticle")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> GetEditById([FromRoute] int id)
        {
            try
            {
                var review = await _service.GetReviewByIdAsync(id);

                return Ok(_mapper.Map<ReviewRequest>(review));
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer[e.Message].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
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

                var created = await _service.CreateReviewAsync(review);
                if (created)
                    return Ok(_localizer["Обзор успешно создан"].Value);

                return BadRequest(_localizer["Не удалось создать обзор"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
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
                    return Ok(_localizer["Обзор успешно обновлен"].Value);

                return BadRequest(_localizer["Не удалось обновить обзор"].Value);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer[e.Message].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize("ChangingArticle")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var deleted = await _service.DeleteReviewAsync(id);
                if (deleted)
                    return NoContent();

                return BadRequest(_localizer["Не удалось удалить обзор"].Value);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer[e.Message].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}