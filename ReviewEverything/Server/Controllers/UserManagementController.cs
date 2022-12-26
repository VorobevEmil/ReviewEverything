using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public UserManagementController(IUserManagementService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
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
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("BlockUser/{userId}")]
        public async Task<IActionResult> BlockUser(string userId, [FromBody] bool statusBlock, CancellationToken token)
        {
            try
            {
                var result = await _service.RefreshStatusBlockAsync(userId, statusBlock, token: token);
                return result ? Ok() : NotFound("Пользователь не найден");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("ChangeUserRole/{userId}")]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] bool statusRole, CancellationToken token)
        {
            try
            {
                await _service.ChangeUserRoleAsync(userId, statusRole, token: token);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete([FromRoute] string userId, CancellationToken token)
        {
            var deleted = await _service.DeleteUserAsync(userId, token);
            if (deleted)
                return NoContent();

            return NotFound("Пользователь не найден");
        }
    }
}