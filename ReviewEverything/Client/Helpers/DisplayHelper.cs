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

    public static string DeclinationEndingUserScope(int count)
    {
        if ((count % 100 > 10 && count % 100 < 20) || (count % 10 >= 5 && count % 10 <= 9) || count % 10 == 0) return "�������";
        else if (count % 10 == 1) return "�����";
        return "������";
    }
}