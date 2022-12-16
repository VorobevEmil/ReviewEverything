using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.ReviewService;
using ReviewEverything.Shared.Contracts.Requests;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

        public CommentController(ICommentService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var comments = await _service.GetCommentByIdAsync(id);
                if (comments == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<CommentResponse>(comments));
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
            return Created(Url.Action($"GetById", new { id = comment.Id })!, _mapper.Map<CommentResponse>(comment));
        }
    }
}