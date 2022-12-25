using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Account;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Пользователь вышел из системы");
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            var applicationUser = await _userManager.FindByEmailAsync(model.Email);
            if (applicationUser != null)
            {
                if (applicationUser.Block)
                    return Conflict("Ваш аккаунт заблокирован");

                var result = await _signInManager.PasswordSignInAsync(applicationUser, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok("Пользователь успешно авторизирован");
                }
            }
            return Conflict("Неправильный логин или пароль, пожалуйста проверьте правильность набора данных");
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null || await _userManager.FindByNameAsync(model.UserName) != null)
            {
                return Conflict("Пользователь уже существует в системе");
            }

            ApplicationUser applicationUser = new() { Email = model.Email, UserName = model.UserName, FullName = model.FullName };

            var result = await _userManager.CreateAsync(applicationUser, model.Password);
            if (result.Succeeded)
            {
                return Ok("Пользователь успешно зарегистрирован");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return StatusCode(500, "Не удалось зарегистрировать пользователя, пожалуйста повторите попытку позже");
        }

        [HttpGet("GetCurrentUserData")]
        public ActionResult<UserInfo> GetCurrentUserData()
        {
            UserInfo userInfo = new();
            if (User.Identity!.IsAuthenticated)
            {
                userInfo.AuthenticationType = User.Identity!.AuthenticationType!;
                userInfo.Claims = User.Claims.Select(t => new ApiClaim(t.Type, t.Value)).ToList();
            }
            return Ok(userInfo);
        }
    }
}
