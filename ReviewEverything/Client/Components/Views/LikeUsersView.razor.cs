using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudBlazor;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ReviewEverything.Client.Components.Views
{
    public partial class LikeUsersView
    {
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private IBreakpointService BreakpointListener { get; set; } = default!;

        [Parameter] public int Id { get; set; }
        [Parameter] public List<string> LikeUsers { get; set; } = default!;
        private ClaimsPrincipal User { get; set; } = default!;
        private Breakpoint _breakpoint = default!;

        private bool _userLike = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var subscriptionResult = await BreakpointListener.Subscribe((breakpoint) =>
                {
                    _breakpoint = breakpoint;
                    InvokeAsync(StateHasChanged);
                }, new ResizeOptions
                {
                    NotifyOnBreakpointOnly = true,
                });

                _breakpoint = subscriptionResult.Breakpoint;
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            User = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            _userLike = CheckUserSetLike();
        }

        private bool CheckUserSetLike()
        {
            return User.Identity!.IsAuthenticated && LikeUsers.Contains(GetUserId());
        }

        private async Task SetUserLikeAsync(bool like)
        {
            if (!User.Identity!.IsAuthenticated)
                return;

            _userLike = like;
            if (like)
            {
                LikeUsers.Add(GetUserId());
                await HttpClient.PostAsJsonAsync("api/UserLike", Id);
            }
            else
            {
                LikeUsers.Remove(GetUserId());
                await HttpClient.DeleteAsync($"api/UserLike/{Id}");
            }
        }

        private string GetUserId()
        {
            return User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}