using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.CategoryService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;
    private readonly IMapper _mapper;

    public CategoryController(ICategoryService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll()
    {
        try
        {
            var categories = await _service.GetCategoriesAsync();
            return Ok(_mapper.Map<List<CategoryResponse>>(categories));
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetById([FromRoute] int id)
    {
        try
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryResponse>(category));
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);

        var result = await _service.CreateCategoryAsync(category);

        return Created(Url.Action($"GetById", new { id = category.Id })!, _mapper.Map<CategoryResponse>(category));
    }

    [HttpPut("{categoryId}")]
    [Authorize("Admin")]
    //TODO: Протестить с добавленными произведениями
    public async Task<IActionResult> Update([FromRoute] int categoryId, [FromBody] CategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        category.Id = categoryId;

        var updated = await _service.UpdateCategoryAsync(category);
        if (updated)
            return Ok(category);

        return NotFound();
    }

    [HttpDelete("{categoryId}")]
    [Authorize("Admin")]
    public async Task<IActionResult> Delete([FromRoute] int categoryId)
    {
        var deleted = await _service.DeleteCategoryAsync(categoryId);
        if (deleted)
            return NoContent();

        return NotFound();
    }
}