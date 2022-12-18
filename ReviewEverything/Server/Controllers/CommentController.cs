using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.ReviewService;
using ReviewEverything.Shared.Contracts.Requests;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        public CommentController(ICommentService service, IMapper mapper, IHubContext<CommentHub> hubContext)
        {
            _service = service;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet("GetByReviewId/{reviewId}")]
        public async Task<ActionResult<List<CommentResponse>>> GetCommentsByReviewId(int reviewId)
        {
            try
            {
                var comments = await _service.GetCommentsByReviewIdAsync(reviewId);

                return Ok(_mapper.Map<List<CommentResponse>>(comments));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CommentRequest request)
        {
            var comment = _mapper.Map<Comment>(request);
            comment.UserId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            comment.CreationDate = DateTime.UtcNow;

            var result = await _service.CreateCommentAsync(comment);
            var commentResponse = _mapper.Map<CommentResponse>(comment);

            await _hubContext.Clients.Group(comment.ReviewId.ToString()).SendAsync("ReceiveComment", commentResponse);
            return Ok();
        }
    }
}