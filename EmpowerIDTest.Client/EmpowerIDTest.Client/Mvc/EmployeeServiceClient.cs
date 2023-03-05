using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using EmpowerIDTest.Client.ViewModels;
using EmpowerIDTest.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;

namespace EmpowerIDTest.Client.Mvc;

internal class EmployeeServiceClient : ServiceClientBase
{
    public EmployeeServiceClient(SettingsViewModel settings) : base(settings)
    {
    }

    //public Task Create(User user)
    //{
    //    return _httpClient!.PostAsync(QueryHelpers.AddQueryString("api/User", user), null);
    //}

    //public Task Register(UserViewModel user)
    //{
    //    return _httpClient!.PutAsync(QueryHelpers.AddQueryString("User/register", new Dictionary<string, string>
    //    {
    //        ["userName"] = user.Name.Trim(),
    //        ["password"] = user.Password?.Trim() ?? "",
    //        ["role"] = user.Role.Trim(),
    //        ["email"] = user.Email.Trim(),
    //        ["phone"] = user.Phone.Trim()
    //    }), null);
    //}

    public async Task<PagedList<Employee>> List(EmployeeSearchRequest? request)
    {
        var response = await _httpClient!.PostAsync("Employee/Search", JsonContent.Create(request));

        var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());

        return new PagedList<Employee>(jObject["result"]?.ToObject<List<Employee>>() ?? new List<Employee>(),
            jObject["fullCount"]?.Value<int>() ?? throw new Exception("No count"), 50, 1);
    }

    public async Task Delete(Employee employee)
    {
        await _httpClient!.DeleteAsync($"Employee/{employee.Id}");
    }

    //public async Task Update(UserViewModel user)
    //{
    //    await _httpClient!.PostAsync(QueryHelpers.AddQueryString("User/update", new Dictionary<string, string>
    //    {
    //        ["userId"] = user.Id.Trim(),
    //        ["password"] = user.Password?.Trim() ?? "",
    //        ["role"] = user.Role.Trim(),
    //        ["email"] = user.Email.Trim(),
    //        ["phone"] = user.Phone.Trim()
    //    }), null, CancellationToken.None);
    //}
}