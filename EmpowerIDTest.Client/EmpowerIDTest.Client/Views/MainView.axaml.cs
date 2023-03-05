using Avalonia.Controls;
using EmpowerIDTest.Client.ViewModels;

namespace EmpowerIDTest.Client.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void OnDataGridSorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is EmployeeListViewModel viewModel) viewModel.ScheduleLoadCommand.Execute(null);
    }
}