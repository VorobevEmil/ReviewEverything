using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Services.UserManagementService;
using ReviewEverything.Shared.Contracts.Responses;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize("Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _service;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<UserManagementController> _localizer;

        public UserManagementController(IUserManagementService service, IMapper mapper, IStringLocalizer<UserManagementController> localizer)
        {
            _service = service;
            _mapper = mapper;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserManagementResponse>>> GetAll(int page, int pageSize, FilterUserByProperty filterUserByProperty, string? search, CancellationToken token)
        {
            try
            {
                var result = await _service.GetUsersAsync(page, pageSize, filterUserByProperty, search, token);
                var users = _mapper.Map<List<UserManagementResponse>>(result.users);
                foreach (var user in users)
                {
                    if (await _service.CheckUserContainsAdminRole(user.Id))
                    {
                        user.Admin = true;
                    }
                }

                return Ok(new UserCountResponse(result.count, users));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("BlockUser/{userId}")]
        public async Task<IActionResult> BlockUser(string userId, [FromBody] bool statusBlock, CancellationToken token)
        {
            try
            {
                var result = await _service.RefreshStatusBlockAsync(userId, statusBlock, token: token);
                if (result)
                    return Ok(_localizer["Статус блокировки пользователя изменен"].Value);

                return BadRequest(_localizer["Не удалось изменить статус блокировки пользователя"].Value);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer["Пользователь не найден"].Value);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("ChangeUserRole/{userId}")]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] bool statusRole, CancellationToken token)
        {
            try
            {
                await _service.ChangeUserRoleAsync(userId, statusRole, token: token);
                return Ok(_localizer["Роль пользователя изменена"].Value);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(_localizer["Пользователь не найден"].Value);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete([FromRoute] string userId, CancellationToken token)
        {
            try
            {
                var deleted = await _service.DeleteUserAsync(userId, token);
                if (deleted)
                    return NoContent();

                return BadRequest();
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
    }
}