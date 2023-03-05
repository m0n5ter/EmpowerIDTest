using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EmpowerIDTest.Client.Mvc;
using EmpowerIDTest.Client.ViewModels;
using EmpowerIDTest.Client.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EmpowerIDTest.Client;

public class App : Application
{
    private readonly ServiceProvider _services;

    public static T GetService<T>() => (Current as App ?? throw new InvalidOperationException("App.Current is not defined"))
        ._services.GetService<T>() ?? throw new NullReferenceException($"Failed to resolve service {typeof(T).Name}");

    public App()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddSingleton<MainViewModel>()
            .AddSingleton(_ => SettingsViewModel.Load())
            .AddSingleton<EmployeeListViewModel>()
            .AddSingleton<EmployeeServiceClient>();

        _services = serviceCollection.BuildServiceProvider();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var mainViewModel = GetService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow {DataContext = mainViewModel};
        }

        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView {DataContext = mainViewModel };
        }

        base.OnFrameworkInitializationCompleted();

        _ = mainViewModel.Initialize();
    }
}