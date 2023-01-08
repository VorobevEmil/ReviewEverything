using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Models;
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
        private readonly IStringLocalizer<TagController> _localizer;

        public TagController(ITagService service, IMapper mapper, IStringLocalizer<TagController> localizer)
        {
            _service = service;
            _mapper = mapper;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<ActionResult<List<TagResponse>>> GetAllBySearch(int page, int pageSize, string? search = default)
        {
            try
            {
                var tags = await _service.GetTagsAsync(page, pageSize, search);
                return Ok(_mapper.Map<List<TagResponse>>(tags));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var tag = await _service.GetTagByIdAsync(id);

                return Ok(_mapper.Map<TagResponse>(tag));
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

        [HttpGet("ExistByName/{title}")]
        public async Task<IActionResult> ExistByName(string title)
        {
            try
            {
                var existsTag = await _service.ExistTagByNameAsync(title);
                if (existsTag)
                    return Ok("Данный тег уже существует");

                return NotFound(_localizer["Тег не найден"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagRequest request)
        {
            try
            {
                var tag = _mapper.Map<Tag>(request);

                var result = await _service.CreateTagAsync(tag);

                if (result)
                    return Created(Url.Action($"GetById", new { id = tag.Id })!, _mapper.Map<TagResponse>(tag));

                return BadRequest();
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(_localizer["Данный тег уже существует"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
