using AspNet.Security.OAuth.Vkontakte;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Account;
using System.Security.Claims;
using Microsoft.Extensions.Localization;

namespace ReviewEverything.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OAuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer<OAuthController> _localizer;

        public OAuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IStringLocalizer<OAuthController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
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
                return Conflict(_localizer["Не удалось войти через поставщика"].Value);
            }

            var userClaims = result.Principal.Claims.ToArray();

            var providerKey = userClaims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var userProvider = await _userManager.FindByLoginAsync(provider, providerKey);
            if (userProvider is not null)
            {
                if (!userProvider.Block)
                    await _signInManager.SignInAsync(userProvider, false, null);
                return Redirect("/");
            }

            return Redirect($"/account/sign-up-provider/{provider}");
        }

        [HttpGet("GetSignUpModel/{provider}")]
        public async Task<ActionResult<SignInModel>> GetSignUpModel(string provider)
        {
            var result = await HttpContext.AuthenticateAsync(provider);
            if (!result.Succeeded)
            {
                return Conflict(_localizer["Не удалось войти через поставщика"].Value);
            }

            var userClaims = result.Principal.Claims.ToArray();
            var givenName = userClaims.First(x => x.Type == ClaimTypes.GivenName).Value;
            var surname = userClaims.First(x => x.Type == ClaimTypes.Surname).Value;
            var fullName = string.Join(" ", givenName, surname);

            OAuthSignUpModel signUp = new OAuthSignUpModel()
            {
                FullName = fullName,
            };
            return Ok(signUp);
        }

        [HttpPost("SignUp/{provider}")]
        public async Task<IActionResult> SignUp(string provider, OAuthSignUpModel model)
        {
            var result = await HttpContext.AuthenticateAsync(provider);
            if (!result.Succeeded)
            {
                return Conflict(_localizer["Не удалось войти через поставщика"].Value);
            }

            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                return Conflict(_localizer["Данное имя пользователя уже существует в системе"].Value);
            }

            ApplicationUser user = new()
            {
                FullName = model.FullName,
                UserName = model.UserName,
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                var userClaims = result.Principal.Claims.ToArray();
                var providerKey = userClaims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                var loginProvider = new UserLoginInfo(provider, providerKey, provider);
                await _userManager.AddLoginAsync(user, loginProvider);
                await _signInManager.SignInAsync(user, false, null);
                return Ok(_localizer["Пользователь успешно зарегистрирован"].Value);
            }

            return Conflict(_localizer["Не удалось зарегистрировать пользователя, пожалуйста повторите попытку позже"].Value);
        }
    }
}