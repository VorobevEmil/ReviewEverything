using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Account;
using System.Threading;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Cors;

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
            if (!ModelState.IsValid)
            {
                return BadRequest("Данные не валидны");
            }

            var applicationUser = await _userManager.FindByEmailAsync(model.Email);
            if (applicationUser != null)
            {
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
            if (!ModelState.IsValid)
            {
                return BadRequest("Данные не валидны");
            }

            if (await _userManager.FindByEmailAsync(model.Email) != null || await _userManager.FindByNameAsync(model.Username) != null)
            {
                return Conflict("Пользователь уже существует в системе");
            }

            ApplicationUser applicationUser = new ApplicationUser { Email = model.Email, UserName = model.Username };

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
            UserInfo userInfo = new UserInfo();
            if (User.Identity!.IsAuthenticated)
            {
                userInfo.AuthenticationType = User.Identity!.AuthenticationType!;
                userInfo.Claims = User.Claims.Select(t => new ApiClaim(t.Type, t.Value)).ToList();
            }
            return Ok(userInfo);
        }

        [HttpGet("SignIn-Google")]
        public IActionResult SignInGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("Google-Response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var email = result.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
                if (email != null)
                {
                    var applicationUser = await _userManager.FindByEmailAsync(email);
                    if (applicationUser != null)
                    {
                        await _signInManager.SignInAsync(applicationUser, false, null);
                        return Redirect("/");
                    }
                    else
                    {
                        var user = new ApplicationUser()
                        {
                            Email = email
                        };
                        await _userManager.CreateAsync(user);
                        await _signInManager.SignInAsync(user, false, null);

                        return Redirect("/account/set-username");
                    }
                }
            }

            return BadRequest("Не удалось войти в систему");
        }
    }
}
