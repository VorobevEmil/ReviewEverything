using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Client.Services.Authorization;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Shared;

public partial class MainLayout
{
    [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    private List<CategoryResponse> Categories { get; set; } = default!;
    private bool IsDarkMode { get; set; } = default!;
    private bool DrawerOpen { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (await LocalStorage.ContainKeyAsync("isDarkMode"))
        {
            IsDarkMode = await LocalStorage.GetItemAsync<bool>("isDarkMode");
        }

        Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
    }

    private async Task ChangeThemeAsync()
    {
        IsDarkMode = !IsDarkMode;
        await LocalStorage.SetItemAsync("isDarkMode", IsDarkMode);
    }

    public void ChangeDrawerOpen()
    {
        DrawerOpen = !DrawerOpen;
    }

    public void RefreshState()
    {
        HostAuthenticationStateProvider.RefreshState();
        StateHasChanged();
    }
}