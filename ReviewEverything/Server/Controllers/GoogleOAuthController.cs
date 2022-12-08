using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class GoogleOAuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public GoogleOAuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("SignIn")]
    public IActionResult SignIn()
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("ReceiveAccountGoogle") };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("ReceiveAccountGoogle")]
    public async Task<IActionResult> ReceiveAccountGoogle()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (result.Succeeded)
        {
            var email = result.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            if (email != null)
            {
                var existUser = await _userManager.FindByEmailAsync(email);
                if (existUser != null)
                {
                    await _signInManager.SignInAsync(existUser, false, null);
                    return Redirect("/");
                }
                else
                {
                    var user = new ApplicationUser()
                    {
                        UserName = email.Split('@').First(),
                        Email = email
                    };
                    await _userManager.CreateAsync(user);
                    await _signInManager.SignInAsync(user, false, null);
                    return Redirect("/");
                }
            }
        }

        return Conflict();
    }
}