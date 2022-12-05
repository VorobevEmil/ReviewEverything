using ReviewEverything.Shared.Models.Account;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ReviewEverything.Client.Services.Authorization
{
    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        public HostAuthenticationStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userInfo = (await _httpClient.GetFromJsonAsync<UserInfo>("api/Account/GetCurrentUserData"))!;
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            if (userInfo.Claims != null)
            {
                claimsIdentity = new ClaimsIdentity(
                    userInfo.Claims.Select(t => new Claim(t.Type, t.Value)).ToList(),
                    userInfo.AuthenticationType);
            }

            return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
        }

        public void RefreshState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
