using EmpowerIDTest.Shared;
using System;
using System.Text.RegularExpressions;

namespace EmpowerIDTest.Client.ViewModels.Dialogs;

public class EmployeeDialogViewModel : DialogViewModelBase
{
    public Employee Employee { get; }

    private string _name;
    private string _password;
    private string? _email;
    private string? _phone;
    private DateTime _dob;
    private string? _department;

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                OnPropertyChanged(nameof(Error));
                OkCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                OnPropertyChanged(nameof(Error));
                OkCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string? Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                OnPropertyChanged(nameof(Error));
                OkCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string? Phone
    {
        get => _phone;
        set
        {
            if (SetProperty(ref _phone, value))
            {
                OnPropertyChanged(nameof(Error));
                OkCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public DateTime DOB
    {
        get => _dob;
        set
        {
            if (SetProperty(ref _dob, value))
            {
                OnPropertyChanged(nameof(Error));
                OkCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string? Department
    {
        get => _department;
        set
        {
            if (SetProperty(ref _department, value))
            {
                OnPropertyChanged(nameof(Error));
                OkCommand.NotifyCanExecuteChanged();}
        }
    }

    public EmployeeDialogViewModel(Employee? employee)
    {
        Title = employee == null ? "New Employee": "Edit Employee";
        Employee = employee ?? new Employee();
        
        _name = Employee.Name ?? "";
        _password = Employee.Password ?? "";
        _email = Employee.Email ?? "";
        _phone = Employee.Phone ?? "";
        _dob = Employee.DOB ?? DateTime.Today.AddYears(-18);
        _department = Employee.Department ?? "";
    }

    public string Title { get; }

    public override string? Error
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Name)) return "Name is required";
            if (Name.Length < 5) return "Name is too short";
            if (Name.Length > 50) return "Name is too long";
            if (string.IsNullOrWhiteSpace(Password)) return "Password is required";
            if (Password.Length < 3) return "Password is too short";
            if (Password.Length > 50) return "Password is too long";
            if (string.IsNullOrWhiteSpace(Email)) return "Email is required";
            if (!Regex.IsMatch(Email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")) return "Email is invalid";
            if (!string.IsNullOrEmpty(Phone) && !Regex.IsMatch(Phone, "^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$")) return "Phone number is invalid";
            if (DOB>=DateTime.Today || DOB < DateTime.Today.AddYears(-120)) return "DOB is invalid";
            if (!string.IsNullOrEmpty(Department) && Department.Length > 50) return "Department name is too long";
            return null;
        }
    }

    protected override void Ok()
    {
        Employee.Name = Name;
        Employee.Password = Password;
        Employee.Email = Email;
        Employee.Phone = Phone;
        Employee.DOB = DOB;
        Employee.Department = Department;

        base.Ok();
    }
}