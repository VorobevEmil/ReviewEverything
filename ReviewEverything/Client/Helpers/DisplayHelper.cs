﻿using MudBlazor;

namespace ReviewEverything.Client.Helpers
{
    public class DisplayHelper
    {
        private readonly IDialogService _dialogService;

        public DisplayHelper(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool?> ShowDeleteMessageBoxAsync()
        {
            return await _dialogService.ShowMessageBox(
                "Внимание",
                "Удаление не может быть отменено!",
                yesText: "Удалить!", cancelText: "Отмена");
        }
    }
}