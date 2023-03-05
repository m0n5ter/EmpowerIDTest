namespace EmpowerIDTest.Client.ViewModels.Dialogs;

public class ConfirmationDialogViewModel: DialogViewModelBase
{
    public ConfirmationDialogViewModel(string message)
    {
        Message = message;
    }

    public string Message { get; }
}