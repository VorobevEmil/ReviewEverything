using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Pages.Admin;

namespace ReviewEverything.Client.Helpers
{
    public class DisplayHelper
    {
        private readonly IDialogService _dialogService;
        private readonly IStringLocalizer<UserManager> _localizer;

        public DisplayHelper(IDialogService dialogService, IStringLocalizer<UserManager> localizer)
        {
            _dialogService = dialogService;
            _localizer = localizer;
        }

        public async Task<bool?> ShowDeleteMessageBoxAsync()
        {
            return await _dialogService.ShowMessageBox(
                _localizer["Warning"],
                _localizer["Delete message"],
                yesText: _localizer["Delete"], cancelText: _localizer["Cancel"]);
        }

        public async Task<bool?> ShowMessageBoxAsync(string message, string yesText, string cancelText = "Cancel")
        {
            cancelText = _localizer[cancelText];
            return await _dialogService.ShowMessageBox(
                _localizer["Warning"],
                message,
                yesText: yesText, cancelText: cancelText);
        }
    }
}