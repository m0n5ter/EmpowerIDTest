using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using EmpowerIDTest.Client.Mvc;
using EmpowerIDTest.Client.Utils;
using EmpowerIDTest.Shared;

namespace EmpowerIDTest.Client.ViewModels;

internal partial class EmployeeListViewModel : ViewModelBase
{
    private readonly Throttler _loadThrottler = new();
    private string? _filterStr;
    private bool _isLoading;
    private readonly EmployeeServiceClient _client;
    private readonly SettingsViewModel _settings;

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

    [RelayCommand(CanExecute = nameof(CanClearFilter))]
    private void ClearFilter() => FilterStr = null;

    public bool CanClearFilter => FilterStr != null;

    public AvaloniaList<Employee> Employees { get; } = new();
    
    [RelayCommand]
    private async Task DeleteEmployee(Employee employee)
    {
        try
        {
            await _client.Delete(employee);
            Employees.Remove(employee);
        }
        catch(Exception e)
        {
            App.GetService<MainViewModel>().Error = e;
        }
    }

    public EmployeeListViewModel(EmployeeServiceClient client, SettingsViewModel settings)
    {
        _client = client;
        _settings = settings;
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

        try
        {
            await Task.Delay(500);
            var employees = await _client.List(new EmployeeSearchRequest
            {
                SearchTerm = FilterStr,
                PageSize = _settings.ItemsPerPage
            });

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Employees.Clear();
                Employees.AddRange(employees);
            });
        }
        catch (Exception e)
        {
            App.GetService<MainViewModel>().Error = e;
        }
        finally
        {
            IsLoading = false;
        }
    }
}