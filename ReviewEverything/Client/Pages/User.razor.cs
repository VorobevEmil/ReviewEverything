using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Pages
{
    public partial class User
    {
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Parameter] public string Id { get; set; } = default!;
        private UserResponse UserResponse { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/User/{Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                UserResponse = (await httpResponseMessage.Content.ReadFromJsonAsync<UserResponse>())!;
            }
        }
    }
}