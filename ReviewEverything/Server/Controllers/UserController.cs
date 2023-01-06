using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Services.UserService;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public UserController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(string id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound("Пользователь не найден");

                return Ok(_mapper.Map<UserResponse>(user));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время поиска пользователя по Id произошла внутренняя ошибка сервера");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditAboutMe([FromBody] string aboutMe)
        {
            try
            {
                var userId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _service.EditAboutMeAsync(userId, aboutMe);
                return result ? Ok() : BadRequest("Не удалось обновить поле \"Обо мне\"");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Во время обновления поля \"Обо мне\" произошла внутренняя ошибка сервера");
            }
        }
    }
}