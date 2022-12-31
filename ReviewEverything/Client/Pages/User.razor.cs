using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using ReviewEverything.Client.Resources;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Pages
{
    public partial class User
    {
        [Parameter] public string Id { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        private IStringLocalizer<ResourcesShared> SharedLocalizer { get; set; } = ResourcesShared.CreateStringLocalizer();

        private UserResponse? UserResponse { get; set; }

        private bool _editor;

        protected override async Task OnInitializedAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/User/{Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                UserResponse = (await httpResponseMessage.Content.ReadFromJsonAsync<UserResponse>())!;
            }

            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.Identity!.IsAuthenticated && UserResponse != null)
            {
                if (user.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value == UserResponse.Id || user.IsInRole("Admin"))
                {
                    _editor = true;
                }
            }
        }
    }
}