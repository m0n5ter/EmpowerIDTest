using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;

namespace EmpowerIDTest.Client.ViewModels;

internal partial class EmployeeViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id = "";

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _email = "";

    [ObservableProperty]
    private string? _password = "";

    [ObservableProperty]
    private string _phone = "";

    [ObservableProperty]
    private DateTime? _dob;

    [ObservableProperty]
    private string? _department;

    public static EmployeeViewModel FromJObject(JObject jObject, string? password)
    {
        return new EmployeeViewModel
        {
            Id = jObject["id"]?.Value<string>() ?? throw new ArgumentException("User ID is missing"),
            Name = jObject["name"]?.Value<string>() ?? throw new ArgumentException("User Name is missing"),
            Password = password,
            Email = jObject["email"]?.Value<string>() ?? string.Empty,
            Phone = jObject["phoneNumber"]?.Value<string>() ?? string.Empty,
        };
    }
}