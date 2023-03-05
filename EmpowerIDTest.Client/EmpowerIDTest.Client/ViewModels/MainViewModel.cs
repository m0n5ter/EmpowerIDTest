using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace EmpowerIDTest.Client.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    public EmployeeListViewModel EmployeeList => App.GetService<EmployeeListViewModel>();

    private Exception? _error;

    public Exception? Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    [RelayCommand]
    private void ClearError() => Error = null;

    public async Task Initialize()
    {
        try
        {
            await EmployeeList.Initialize();
        }
        catch (Exception exception)
        {
            Error = exception;
        }
    }
}