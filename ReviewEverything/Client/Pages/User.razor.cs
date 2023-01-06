using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Client.Resources;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Pages
{
    public partial class User
    {
        [Parameter] public string Id { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private IStringLocalizer<User> Localizer { get; set; } = default!;
        private IStringLocalizer<ResourcesShared> SharedLocalizer { get; set; } = ResourcesShared.CreateStringLocalizer();

        private UserResponse? UserResponse { get; set; }
        private bool _editor;
        private string? _editAboutMe;
        private string? _userId;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await GetUserFromApiAsync();
                await CheckUserAsync();
            }
            catch
            {
                await DisplayHelper.ShowErrorResponseMessage();
            }

        }

        private async Task GetUserFromApiAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/User/{Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                UserResponse = (await httpResponseMessage.Content.ReadFromJsonAsync<UserResponse>())!;
            }
            else
            {
                throw new Exception();
            }
        }

        private async Task CheckUserAsync()
        {
            await GetUserIdAsync();
            await CheckCanUserEditPageAsync();
        }

        private async Task GetUserIdAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            _userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        private async Task CheckCanUserEditPageAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if ((_userId is not null && _userId == UserResponse!.Id) || user.IsInRole("Admin"))
            {
                _editor = true;
            }
        }

        private void SetValueFieldEditAboutMe()
        {
            _editAboutMe = string.IsNullOrEmpty(UserResponse!.AboutMe) ? string.Empty : UserResponse.AboutMe;
        }

        private async Task SaveAboutMeAsync()
        {
            var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/User", _editAboutMe);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                UserResponse!.AboutMe = _editAboutMe;
                _editAboutMe = default;
            }
            else
            {
                Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
            }
        }
    }
}