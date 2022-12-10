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
        private UserResponse? UserResponse { get; set; }
        private List<ReviewResponse> Reviews { get; set; } = default!;
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

        private async Task GetReviewsFromApiAsync(int? categoryId)
        {
            Reviews = null!;
            await Task.CompletedTask;
        }
    }
}