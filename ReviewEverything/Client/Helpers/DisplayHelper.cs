using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Resources;

namespace ReviewEverything.Client.Helpers
{
    public class DisplayHelper
    {
        private readonly IDialogService _dialogService;
        private readonly IStringLocalizer<ResourcesDisplayHelper> _localizer = ResourcesDisplayHelper.CreateStringLocalizer();

        public DisplayHelper(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool?> ShowDeleteMessageBoxAsync(string message = "Удаление не может быть отменено!")
        {
            return await _dialogService.ShowMessageBox(
                _localizer["Внимание"],
                _localizer[message],
                yesText: _localizer["Удалить"], cancelText: _localizer["Отмена"]);
        }

        public async Task<bool?> ShowMessageBoxAsync(string message, string yesText = "Да", string cancelText = "Отмена")
        {
            cancelText = _localizer[cancelText];
            return await _dialogService.ShowMessageBox(
                _localizer["Внимание"],
                message,
                yesText: yesText, cancelText: cancelText);
        }
    }
}