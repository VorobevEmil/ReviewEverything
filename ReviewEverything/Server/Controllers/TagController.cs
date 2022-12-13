using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.CompositionService;
using ReviewEverything.Server.Services.TagService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _service;
        private readonly IMapper _mapper;

        public TagController(ITagService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TagResponse>>> GetAll(string? search = default)
        {
            try
            {
                var tags = await _service.GetTagsAsync(search);
                return Ok(_mapper.Map<List<TagResponse>>(tags));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var tag = await _service.GetTagByIdAsync(id);
                if (tag == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<TagResponse>(tag));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagRequest request)
        {
            var tag = _mapper.Map<Tag>(request);

            var result = await _service.CreateTagAsync(tag);

            return Created(Url.Action($"GetById", new { id = tag.Id })!, tag);
        }

        [HttpPut("{tagId}")]
        public async Task<IActionResult> Update([FromRoute] int tagId, [FromBody] TagRequest request)
        {
            var tag = _mapper.Map<Tag>(request);
            tag.Id = tagId;

            var updated = await _service.UpdateTagAsync(tag);
            if (updated)
                return Ok(tag);

            return NotFound();
        }

        [HttpDelete("{tagId}")]
        public async Task<IActionResult> Delete([FromRoute] int tagId)
        {
            var deleted = await _service.DeleteTagAsync(tagId);
            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}
