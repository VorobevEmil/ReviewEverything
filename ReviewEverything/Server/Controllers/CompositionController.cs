using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.CompositionService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompositionController : ControllerBase
{
    private readonly ICompositionService _service;
    private readonly IMapper _mapper;

    public CompositionController(ICompositionService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompositionResponse>>> GetAll(string? search, int? categoryId, string? userId)
    {
        try
        {
            var compositions = await _service.GetCompositionsAsync(search, categoryId, userId);
            return Ok(_mapper.Map<List<CompositionResponse>>(compositions));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Во время получении произведений произошла внутренняя ошибка сервера");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompositionResponse>> GetById([FromRoute] int id)
    {
        try
        {
            var composition = await _service.GetCompositionByIdAsync(id);
            if (composition == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CompositionResponse>(composition));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Во время получения произведения произошла внутренняя ошибка сервера");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CompositionRequest request)
    {
        try
        {
            var composition = _mapper.Map<Composition>(request);

            var result = await _service.CreateCompositionAsync(composition);
            if (result)
            return Created(Url.Action($"GetById", new { id = composition.Id })!, _mapper.Map<CompositionResponse>(composition));

            return BadRequest("Не удалось создать произведение, повторите попытку позже");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Во время создания произведения произошла внутренняя ошибка сервера");

        }
    }
}