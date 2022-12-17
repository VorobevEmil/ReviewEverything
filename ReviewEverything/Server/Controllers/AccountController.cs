using System.Security.Claims;
using AspNet.Security.OAuth.Vkontakte;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Account;
using ReviewEverything.Client.Pages;

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

        [HttpPost("SignIn-Google")]
        public IActionResult SignInGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("ReceiveAccount", new { provider = GoogleDefaults.AuthenticationScheme }) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpPost("SignIn-Vkontakte")]
        public IActionResult SignInVkontakte()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("ReceiveAccount", new { provider = VkontakteAuthenticationDefaults.AuthenticationScheme }) };
            return Challenge(properties, VkontakteAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("ReceiveAccount")]
        public async Task<IActionResult> ReceiveAccount(string provider)
        {
            var result = await HttpContext.AuthenticateAsync(provider);
            if (!result.Succeeded)
            {
                return Conflict($"Не удалось войти через поставщика");
            }

            var userClaims = result.Principal.Claims.ToArray();

            var providerKey = userClaims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var userProvider = await _userManager.FindByLoginAsync(provider, providerKey);
            if (userProvider is not null)
            {
                await _signInManager.SignInAsync(userProvider, false, null);
                return Redirect("/");

            }

            ApplicationUser? user = null;
            var email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email is not null)
            {
                user = await _userManager.FindByEmailAsync(email);
            }


            if (user is null)
            {
                var givenName = userClaims.First(x => x.Type == ClaimTypes.GivenName).Value;
                var surname = userClaims.First(x => x.Type == ClaimTypes.Surname).Value;
                var fullName = string.Join(" ", givenName, surname);

                user = new ApplicationUser()
                {
                    FullName = fullName,
                    Email = email,
                    UserName = "Unknown"
                };

                await _userManager.CreateAsync(user);
            }

            var loginProvider = new UserLoginInfo(provider, providerKey, provider);
            await _userManager.AddLoginAsync(user, loginProvider);
            await _signInManager.SignInAsync(user, false, null);

            return Redirect("/");
        }
    }
}
