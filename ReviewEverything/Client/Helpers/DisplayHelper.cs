using MudBlazor;

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
            "��������",
            "�������� �� ����� ���� ��������!",
            yesText: "�������!", cancelText: "������");
    }
}