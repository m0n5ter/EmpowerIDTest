using System;
using System.ComponentModel;
using System.Linq;
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
    private string _sort = "a_name";

    public EmployeeListViewModel(EmployeeServiceClient client, SettingsViewModel settings)
    {
        _client = client;
        _settings = settings;

        View = new DataGridCollectionView(Employees)
        {
            SortDescriptions =
            {
                DataGridSortDescription.FromPath(nameof(Employee.Name))
            }
        };

        View.SortDescriptions.CollectionChanged += (sender, args) =>
        {
            if ((sender as DataGridSortDescriptionCollection)?.FirstOrDefault() is { } sortDescription)
                Sort = $"{(sortDescription.Direction == ListSortDirection.Ascending ? "a" : "d")}_{sortDescription.PropertyPath}";
        };
    }

    public DataGridCollectionView View { get; }

    public string Sort
    {
        get => _sort;
        set
        {
            if (SetProperty(ref _sort, value))
                ScheduleLoad();
        }
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
        if (CurrentPage != null)
            await Load(CurrentPage.PageNumber + 1);
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
                var newEmployee = new Employee
                {
                    Name = dialog.Name,
                    Password = dialog.Password,
                    Email = dialog.Email,
                    Phone = dialog.Phone,
                    DOB = dialog.DOB,
                    Department = dialog.Department,
                };

                await _client.Create(newEmployee);
                await Dispatcher.UIThread.InvokeAsync(() => Employees.Add(newEmployee));
                SelectedEmployee = newEmployee;
            }
        }
        catch (Exception exception)
        {
            Error = exception;
        }
    }

    [RelayCommand]
    private async Task EditEmployee(Employee employee)
    {
        Error = null;

        try
        {
            var dialog = new EmployeeDialogViewModel(employee);

            if (await App.GetService<MainViewModel>().ShowDialog(dialog))
            {
                employee.Name = dialog.Name;
                employee.Password = dialog.Password;
                employee.Email = dialog.Email;
                employee.Phone = dialog.Phone;
                employee.DOB = dialog.DOB;
                employee.Department = dialog.Department;

                await _client.Update(employee);
                await Dispatcher.UIThread.InvokeAsync(() => View.Refresh());
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
        _loadThrottler.Next(200, (_, _) => Load(1));
    }

    private async Task Load(int pageNumber)
    {
        IsLoading = true;
        Error = null;
        var selectedId = SelectedEmployee?.Id;

        try
        {
            var employees = await _client.List(new EmployeeSearchRequest
            {
                SearchTerm = FilterStr,
                Sort = Sort,
                PageSize = _settings.ItemsPerPage,
                PageNumber = pageNumber
            });

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentPage = employees;
                if (pageNumber == 1) Employees.Clear();
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