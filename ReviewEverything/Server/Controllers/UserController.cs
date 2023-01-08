using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
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
        private readonly IStringLocalizer<UserController> _localizer;

        public UserController(IUserService service, IMapper mapper, IStringLocalizer<UserController> localizer)
        {
            _service = service;
            _mapper = mapper;
            _localizer = localizer;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(string id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);

                return Ok(_mapper.Map<UserResponse>(user));
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer["Пользователь не найден"].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
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
                return result ? Ok() : BadRequest(_localizer["Не удалось обновить поле \"Обо мне\""].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}