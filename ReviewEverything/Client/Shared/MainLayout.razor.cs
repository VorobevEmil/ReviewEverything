using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReviewEverything.Client.Services.Authorization;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Shared;

public partial class MainLayout
{
    [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
    private bool IsDarkMode { get; set; } = default!;
    private bool DrawerOpen { get; set; } = default!;

    private MudAutocomplete<ReviewSearchResponse> _searchAutocomplete = default!;

    private bool _searchDialogOpen;
    private void OpenSearchDialog() => _searchDialogOpen = true;
    private DialogOptions _dialogOptions = new() { Position = DialogPosition.TopCenter, NoHeader = true };

    protected override async Task OnInitializedAsync()
    {
        if (await LocalStorage.ContainKeyAsync("isDarkMode"))
            IsDarkMode = await LocalStorage.GetItemAsync<bool>("isDarkMode");
    }

    private async Task ChangeThemeAsync()
    {
        IsDarkMode = !IsDarkMode;
        await LocalStorage.SetItemAsync("isDarkMode", IsDarkMode);
    }

    public void ChangeDrawerOpen() => DrawerOpen = !DrawerOpen;
    public void RefreshState()
    {
        HostAuthenticationStateProvider.RefreshState();
        StateHasChanged();
    }

    private async Task<IEnumerable<ReviewSearchResponse>> Search(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return new List<ReviewSearchResponse>();

        return (await HttpClient.GetFromJsonAsync<IEnumerable<ReviewSearchResponse>>($"api/Review/Search/{search}"))!;
    }

    private async Task OnSearchResult(ReviewSearchResponse entry)
    {
        NavigationManager.NavigateTo($"/");
        NavigationManager.NavigateTo($"/Article/{entry.Id}");
        await _searchAutocomplete.Clear();
    }
}