using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Shared.Contracts.Responses;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Client.Pages.Admin
{
    public partial class UserManager
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        private HashSet<UserManagementResponse> _selectedUsers = new();
        private MudTable<UserManagementResponse> _mudTable = default!;
        private string _userId = default!;
        private FilterUserByProperty _filterUserByProperty = default!;
        private string _search = default!;

        protected override async Task OnInitializedAsync()
        {
            _userId = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Claims.First(x =>
                x.Type == ClaimTypes.NameIdentifier).Value;
        }

        private async Task<TableData<UserManagementResponse>> ServerReload(TableState state)
        {
            var parameterUrl = ParameterUrl(state.Page, state.PageSize);
            UserCountResponse data = (await HttpClient.GetFromJsonAsync<UserCountResponse>($"api/UserManagement?{parameterUrl}"))!;

            return new TableData<UserManagementResponse>() { TotalItems = data.Count, Items = data.Users };
        }

        private string ParameterUrl(int tablePage, int tablePageSize)
        {
            string page = $"page={tablePage + 1}";
            string pageSize = $"&pageSize={tablePageSize}";
            string filterUserByProperty = $"&filterUserByProperty={_filterUserByProperty}";
            string? search = !string.IsNullOrWhiteSpace(_search) ? $"&search={_search}" : null;
            return page + pageSize + filterUserByProperty + search;
        }

        private async Task SearchUserAsync(string search)
        {
            _search = search;
            await _mudTable.ReloadServerData();
        }

        private async Task ChangeFilterForSearchAsync(FilterUserByProperty filterUserByProperty)
        {
            _search = default!;
            _filterUserByProperty = filterUserByProperty;
            await _mudTable.ReloadServerData();
        }


        private async Task ChangeStatusBlockInUsersAsync(bool statusBlock)
        {

            var status = statusBlock ? "Заблокировать" : "Разблокировать";
            var result = await DisplayHelper.ShowMessageBoxAsync($"Вы действительно хотите {status.ToLower()} выделенных пользователей?", status + "!");
            if (result != true)
                return;

            MoveUser();
            foreach (var user in _selectedUsers)
            {
                var httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/UserManagement/BlockUser/{user.Id}", statusBlock);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    user.Status = statusBlock ? "Заблокирован" : "Разблокирован";
                }
                else
                {
                    Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
                }
            }

            _selectedUsers.Clear();
        }

        private async Task DeleteUsersAsync()
        {

            var result = await DisplayHelper.ShowDeleteMessageBoxAsync();
            if (result != true)
                return;

            MoveUser();
            foreach (var user in _selectedUsers)
            {
                var httpResponseMessage = await HttpClient.DeleteAsync($"api/UserManagement/{user.Id}");
                if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    _selectedUsers.Remove(user);
                else
                    Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
            }

            _selectedUsers.Clear();
            await _mudTable.ReloadServerData();
        }

        private async Task ChangeUserRoleAsync(bool statusRole)
        {
            var status = statusRole ? "выдать" : "забрать";
            var result = await DisplayHelper.ShowMessageBoxAsync($"Вы действительно хотите {status} права администратора выбранным пользователям?", "Да!");
            if (result != true)
                return;

            MoveUser();
            foreach (var user in _selectedUsers)
            {
                var httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/UserManagement/ChangeUserRole/{user.Id}", statusRole);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    user.Admin = statusRole;
                }
                else
                {
                    Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
                }
            }

            _selectedUsers.Clear();
        }

        private void MoveUser()
        {
            if (_selectedUsers.Any(x => x.Id == _userId))
            {
                var user = _selectedUsers.First(x => x.Id == _userId);
                _selectedUsers.Remove(user);
                _selectedUsers.Add(user);
            }
        }
    }
}