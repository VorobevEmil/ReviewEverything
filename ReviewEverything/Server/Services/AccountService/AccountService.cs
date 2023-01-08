using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Account;

namespace ReviewEverything.Server.Services.AccountService;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task SignInAsync(SignInModel model)
    {
        var applicationUser = await _userManager.FindByEmailAsync(model.Email);
        if (applicationUser != null)
        {
            if (applicationUser.Block)
                throw new HttpStatusRequestException(HttpStatusCode.Conflict, "Ваш аккаунт заблокирован");

            var result = await _signInManager.PasswordSignInAsync(applicationUser, model.Password, false, false);
            if (result.Succeeded)
                return;
        }

        throw new HttpStatusRequestException(HttpStatusCode.BadRequest, "Неправильный логин или пароль, пожалуйста проверьте правильность набора данных");
    }

    public async Task SignUpAsync(SignUpModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) != null)
        {
            throw new HttpStatusRequestException(HttpStatusCode.Conflict,
                "Пользователь с таким Email уже существует в системе");
        }

        if (await _userManager.FindByNameAsync(model.UserName) != null)
        {
            throw new HttpStatusRequestException(HttpStatusCode.Conflict,
                "Пользователь с таким именем пользователя уже существует в системе");
        }

        ApplicationUser applicationUser = new() { Email = model.Email, UserName = model.UserName, FullName = model.FullName };

        var result = await _userManager.CreateAsync(applicationUser, model.Password);
        if (result.Succeeded)
        {
            return;
        }

        throw new HttpStatusRequestException(HttpStatusCode.BadRequest, "Не удалось зарегистрировать пользователя, пожалуйста повторите попытку позже");
    }

    public UserInfo GetCurrentUserData(ClaimsPrincipal user)
    {
        UserInfo userInfo = new();
        if (user.Identity!.IsAuthenticated)
        {
            userInfo.AuthenticationType = user.Identity!.AuthenticationType!;
            userInfo.Claims = user.Claims.Select(t => new ApiClaim(t.Type, t.Value)).ToList();
        }

        return userInfo;
    }
}