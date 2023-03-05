using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace EmpowerIDTest.Client.ViewModels.Dialogs;

public class DialogViewModelBase : ViewModelBase
{
    private readonly TaskCompletionSource<bool> _completion = new();

    public RelayCommand OkCommand { get; }

    public RelayCommand CancelCommand { get; }

    public virtual string? Error => null;

    public DialogViewModelBase()
    {
        OkCommand = new RelayCommand(Ok, () => Error == null);
        CancelCommand = new RelayCommand(Cancel);
    }

    protected virtual void Ok()
    {
        _completion.SetResult(true);
    }

    protected virtual void Cancel()
    {
        _completion.SetResult(false);
    }

    public Task<bool> WaitAsync() => _completion.Task;
}