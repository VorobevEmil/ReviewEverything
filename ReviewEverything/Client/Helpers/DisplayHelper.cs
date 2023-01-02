using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Resources;

namespace ReviewEverything.Client.Helpers
{
    public class DisplayHelper
    {
        private readonly IDialogService _dialogService;
        private readonly NavigationManager _navigationManager;
        private readonly IStringLocalizer<DisplayHelper> _localizer;

        public DisplayHelper(IDialogService dialogService, IStringLocalizer<DisplayHelper> localizer, NavigationManager navigationManager)
        {
            _dialogService = dialogService;
            _localizer = localizer;
            _navigationManager = navigationManager;
        }

        public async Task<bool?> ShowDeleteMessageBoxAsync(string message = "Удаление не может быть отменено!")
        {
            return await _dialogService.ShowMessageBox(
                _localizer["Внимание"],
                _localizer[message],
                yesText: _localizer["Удалить"], cancelText: _localizer["Отмена"]);
        }

        public async Task<bool?> ShowMessageBoxAsync(string message, string yesText = "Да", string? cancelText = "Отмена")
        {
            cancelText = cancelText != null ? _localizer[cancelText] : null!;
            return await _dialogService.ShowMessageBox(
                _localizer["Внимание"],
                message,
                yesText: _localizer[yesText], cancelText: cancelText);
        }

        public async Task ShowErrorResponseMessage()
        {
            await ShowMessageBoxAsync("Не удалось обработать запрос, повторите попытку позже", "ОК", null);
            _navigationManager.NavigateTo("./");
        }
    }
}