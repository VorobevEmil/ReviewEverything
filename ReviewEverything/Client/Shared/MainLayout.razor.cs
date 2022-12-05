using Microsoft.AspNetCore.Components;
using ReviewEverything.Client.Services.Authorization;

namespace ReviewEverything.Client.Shared;

public partial class MainLayout
{
    [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
    private bool IsDarkMode { get; set; } = default!;
    private bool DrawerOpen { get; set; } = default!;

    public void RefreshState()
    {
        HostAuthenticationStateProvider.RefreshState();
        StateHasChanged();
    }
}