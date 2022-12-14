using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.ReviewService;
using ReviewEverything.Shared.Contracts.Requests;
using System.Security.Claims;
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

        //[HttpGet]
        //public async Task<ActionResult<List<CommentResponse>>> GetAll()
        //{
        //    try
        //    {
        //        var comments = await _service.GetCommentsAsync();
        //        return Ok(_mapper.Map<List<CommentResponse>>(comments));
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

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
        public async Task<IActionResult> Create([FromBody] CommentRequest request)
        {
            var comment = _mapper.Map<Comment>(request);
            comment.UserId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            comment.CreationDate = DateTime.UtcNow;

            var result = await _service.CreateCommentAsync(comment);
            return Created(Url.Action($"GetById", new { id = comment.Id })!, _mapper.Map<CommentResponse>(comment));
        }

        //[HttpPut("{commentId}")]
        //public async Task<IActionResult> Update([FromRoute] int commentId, [FromBody] CommentRequest request)
        //{
        //    var comments = _mapper.Map<Comment>(request);
        //    comments.Id = commentId;

        //    var updated = await _service.UpdateCommentAsync(comments);
        //    if (updated)
        //        return Ok(comments);

        //    return NotFound();
        //}

        //[HttpDelete("{commentId}")]
        //public async Task<IActionResult> Delete([FromRoute] int commentId)
        //{
        //    var deleted = await _service.DeleteCommentAsync(commentId);
        //    if (deleted)
        //        return NoContent();

        //    return NotFound();
        //}
    }
}