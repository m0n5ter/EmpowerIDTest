using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using EmpowerIDTest.Client.Mvc;
using EmpowerIDTest.Client.Utils;
using EmpowerIDTest.Client.ViewModels.Dialogs;
using EmpowerIDTest.Shared;

namespace EmpowerIDTest.Client.ViewModels;

internal partial class EmployeeListViewModel : ViewModelBase
{
    private readonly EmployeeServiceClient _client;
    private readonly Throttler _loadThrottler = new();
    private readonly SettingsViewModel _settings;
    private PagedList<Employee>? _currentPage;
    private Exception? _error;
    private string? _filterStr;
    private bool _isLoading;
    private Employee? _selectedEmployee;

    public EmployeeListViewModel(EmployeeServiceClient client, SettingsViewModel settings)
    {
        _client = client;
        _settings = settings;
    }

    public Exception? Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    public PagedList<Employee>? CurrentPage
    {
        get => _currentPage;
        private set
        {
            if(SetProperty(ref _currentPage, value))
                LoadMoreCommand.NotifyCanExecuteChanged();
        }
    }

    public string? FilterStr
    {
        get => _filterStr;
        set
        {
            if (SetProperty(ref _filterStr, value))
            {
                ClearFilterCommand.NotifyCanExecuteChanged();
                ScheduleLoad();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool CanClearFilter => FilterStr != null;

    public AvaloniaList<Employee> Employees { get; } = new();

    public Employee? SelectedEmployee
    {
        get => _selectedEmployee;
        set => SetProperty(ref _selectedEmployee, value);
    }

    [RelayCommand(CanExecute = nameof(CanClearFilter))]
    private void ClearFilter()
    {
        FilterStr = null;
    }

    [RelayCommand]
    private void Refresh()
    {
        ScheduleLoad();
    }

    [RelayCommand(CanExecute = nameof(CanLoadMore))]
    private async Task LoadMore()
    {
        IsLoading = true;
        Error = null;

        try
        {
            var employees = await _client.List(new EmployeeSearchRequest
            {
                SearchTerm = FilterStr,
                PageSize = _settings.ItemsPerPage,
                PageNumber = CurrentPage!.PageNumber + 1
            });

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentPage = employees;
                Employees.AddRange(employees);
            });
        }
        catch (Exception exception)
        {
            Error = exception;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public bool CanLoadMore => CurrentPage != null && CurrentPage.FilteredCount > Employees.Count;

    [RelayCommand]
    private async Task AddEmployee()
    {
        Error = null;

        try
        {
            var dialog = new EmployeeDialogViewModel(null);

            if (await App.GetService<MainViewModel>().ShowDialog(dialog))
            {
                await _client.Create(dialog.Employee);
                await Dispatcher.UIThread.InvokeAsync(() => Employees.Add(dialog.Employee));
                SelectedEmployee = dialog.Employee;
            }
        }
        catch (Exception exception)
        {
            Error = exception;
        }
    }
    
    [RelayCommand]
    private async Task DeleteEmployee(Employee employee)
    {
        if (await App.GetService<MainViewModel>().ShowDialog(new ConfirmationDialogViewModel("Are you sure you want to delete selected Employee?")))
        {
            Error = null;

            try
            {
                await _client.Delete(employee);
                await Dispatcher.UIThread.InvokeAsync(() => Employees.Remove(employee));
            }
            catch (Exception exception)
            {
                Error = exception;
            }
        }
    }

    public Task Initialize()
    {
        ScheduleLoad();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void ScheduleLoad()
    {
        _loadThrottler.Next(200, Load);
    }

    private async Task Load(string? _, CancellationToken ct)
    {
        IsLoading = true;
        Error = null;
        var selectedId = SelectedEmployee?.Id;

        try
        {
            var employees = await _client.List(new EmployeeSearchRequest
            {
                SearchTerm = FilterStr,
                PageSize = _settings.ItemsPerPage
            });

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentPage = employees;
                Employees.Clear();
                Employees.AddRange(employees);
                SelectedEmployee = employees.FirstOrDefault(_ => _.Id == selectedId);
            });
        }
        catch (Exception exception)
        {
            Error = exception;
        }
        finally
        {
            IsLoading = false;
        }
    }
}