using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services;
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
    public async Task<ActionResult<List<CompositionResponse>>> GetAll(string? search)
    {
        try
        {
            var compositions = await _service.GetCompositionsAsync(search);
            return Ok(_mapper.Map<List<CompositionResponse>>(compositions));
        }
        catch
        {
            return BadRequest();
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
            return BadRequest();
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CompositionRequest request)
    {
        var composition = _mapper.Map<Composition>(request);

        var result = await _service.CreateCompositionAsync(composition);

        return Created(Url.Action($"GetById", new { id = composition.Id })!, _mapper.Map<CompositionResponse>(composition));
    }

    [HttpPut("{compositionId}")]
    [Authorize("Admin")]
    public async Task<IActionResult> Update([FromRoute] int compositionId, [FromBody] CompositionRequest request)
    {
        var composition = _mapper.Map<Composition>(request);
        composition.Id = compositionId;

        var updated = await _service.UpdateCompositionAsync(composition);
        if (updated)
            return Ok(composition);

        return NotFound();
    }

    [HttpDelete("{compositionId}")]
    [Authorize("Admin")]
    public async Task<IActionResult> Delete([FromRoute] int compositionId)
    {
        var deleted = await _service.DeleteCompositionAsync(compositionId);
        if (deleted)
            return NoContent();

        return NotFound();
    }
}