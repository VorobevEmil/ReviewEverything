using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Services.AccountService;
using ReviewEverything.Shared.Models.Account;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(IAccountService service, IStringLocalizer<AccountController> localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return Ok(_localizer["Пользователь вышел из системы"].Value);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            try
            {
                await _service.SignInAsync(model);
                return Ok(_localizer["Пользователь успешно авторизирован"].Value);
            }
            catch (HttpStatusRequestException e) when(e.StatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(_localizer[e.Message].Value);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(_localizer[e.Message].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            try
            {
                await _service.SignUpAsync(model);
                return Ok(_localizer["Пользователь успешно зарегистрирован"]);
            }
            catch (HttpStatusRequestException e) when (e.StatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(_localizer[e.Message].Value);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(_localizer[e.Message].Value);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetCurrentUserData")]
        public ActionResult<UserInfo> GetCurrentUserData()
        {
            return Ok(_service.GetCurrentUserData(User));
        }
    }
}
