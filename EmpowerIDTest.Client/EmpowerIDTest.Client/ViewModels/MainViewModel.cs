using System;
using System.Threading.Tasks;
using EmpowerIDTest.Client.ViewModels.Dialogs;

namespace EmpowerIDTest.Client.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    private DialogViewModelBase? _dialog;

    public EmployeeListViewModel EmployeeList => App.GetService<EmployeeListViewModel>();

    public DialogViewModelBase? Dialog
    {
        get => _dialog;
        private set => SetProperty(ref _dialog, value);
    }

    public async Task<bool> ShowDialog(DialogViewModelBase dialog)
    {
        Dialog = dialog;

        try
        {
            return await dialog.WaitAsync();
        }
        finally
        {
            Dialog = null;
        }
    }

    public async Task Initialize()
    {
        await EmployeeList.Initialize();
    }
}