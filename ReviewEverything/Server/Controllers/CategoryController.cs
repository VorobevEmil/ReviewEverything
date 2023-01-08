using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Models;
using ReviewEverything.Server.Services.CategoryService;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CategoryController> _localizer;

        public CategoryController(ICategoryService service, IMapper mapper, IStringLocalizer<CategoryController> localizer)
        {
            _service = service;
            _mapper = mapper;
            _localizer = localizer;
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
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var category = await _service.GetCategoryByIdAsync(id);
                return Ok(_mapper.Map<CategoryResponse>(category));
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

        [HttpPost]
        [Authorize("Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryRequest request)
        {
            try
            {
                var category = _mapper.Map<Category>(request);
                var created = await _service.CreateCategoryAsync(category);
                if (created)
                    return Created(Url.Action($"GetById", new { id = category.Id })!, _mapper.Map<CategoryResponse>(category));

                return BadRequest(_localizer["Не удалось создать категорию"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{categoryId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> Update([FromRoute] int categoryId, [FromBody] CategoryRequest request)
        {
            try
            {
                var category = _mapper.Map<Category>(request);
                category.Id = categoryId;

                var updated = await _service.UpdateCategoryAsync(category);
                if (updated)
                    return Ok(category);

                return BadRequest(_localizer["Не удалось обновить категорию"].Value);
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

        [HttpDelete("{categoryId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> Delete([FromRoute] int categoryId)
        {
            try
            {
                var deleted = await _service.DeleteCategoryAsync(categoryId);
                if (deleted)
                    return NoContent();

                return BadRequest(_localizer["Не удалось удалить категорию"].Value);
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