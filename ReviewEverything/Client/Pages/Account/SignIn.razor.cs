using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using ReviewEverything.Shared.Models.Account;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Client.Services.Authorization;
using ReviewEverything.Client.Shared;

namespace ReviewEverything.Client.Pages.Account;

public partial class SignIn
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
    [CascadingParameter] public MainLayout Parent { get; set; } = default!;

    private readonly SignInModel _model = new();
    private bool _sendRequest = false;

    private async Task OnValidSubmitAsync(EditContext context)
    {
        _sendRequest = true;

        var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Account/SignIn", _model);
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            Snackbar.Add("Пользователь успешно авторизован", Severity.Success);
            HostAuthenticationStateProvider.RefreshState();
            await OnInitializedAsync();
            Parent.RefreshState();
            NavigationManager.NavigateTo("/");
        }
        else
        {
            Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
        }

        _sendRequest = false;
    }
}