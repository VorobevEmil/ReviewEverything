using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Hubs;
using ReviewEverything.Server.Services.CommentService;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;
        private readonly IMapper _mapper;
        private readonly IHubContext<CommentHub> _hubContext;
        private readonly IStringLocalizer<CommentController> _localizer;

        public CommentController(ICommentService service, IMapper mapper, IHubContext<CommentHub> hubContext, IStringLocalizer<CommentController> localizer)
        {
            _service = service;
            _mapper = mapper;
            _hubContext = hubContext;
            _localizer = localizer;
        }

        [HttpGet("GetByReviewId/{reviewId}")]
        public async Task<ActionResult<List<CommentResponse>>> GetCommentsByReviewId(int reviewId, int pageNumber, int pageSize, int elementSkip)
        {
            try
            {
                var comments = await _service.GetCommentsByReviewIdAsync(reviewId, pageNumber, pageSize, elementSkip);

                return Ok(_mapper.Map<List<CommentResponse>>(comments));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CommentRequest request)
        {
            try
            {
                var comment = _mapper.Map<Comment>(request);
                comment.UserId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                comment.CreationDate = DateTime.UtcNow;

                var result = await _service.CreateCommentAsync(comment);
                if (result)
                {
                    var commentResponse = _mapper.Map<CommentResponse>(comment);

                    await _hubContext.Clients.Group(comment.ReviewId.ToString()).SendAsync("ReceiveComment", commentResponse);
                    return Ok(_localizer["Комментарии под обзором успешно создан"].Value);
                }

                return BadRequest(_localizer["Не удалось создать комментарии, повторите попытку позже"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}