using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EmpowerIDTest.Client.ViewModels;
using EmpowerIDTest.Shared;
using Newtonsoft.Json.Linq;

namespace EmpowerIDTest.Client.Mvc;

internal class EmployeeServiceClient : ServiceClientBase
{
    public EmployeeServiceClient(SettingsViewModel settings) : base(settings)
    {
    }

    public async Task<PagedList<Employee>> List(EmployeeSearchRequest? request)
    {
        var response = await _httpClient!.PostAsync("Employee/Search", JsonContent.Create(request));

        var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());

        return new PagedList<Employee>(jObject["result"]?.ToObject<List<Employee>>() ?? new List<Employee>(),
            jObject["fullCount"]?.Value<int>() ?? throw new Exception("No fullCount"),
            jObject["filteredCount"]?.Value<int>() ?? throw new Exception("No filteredCount"),
            request?.PageSize ?? 50, request?.PageNumber ?? 1);
    }

    public async Task Create(Employee employee)
    {
        await _httpClient!.PostAsync("Employee", JsonContent.Create(employee));
    }

    public async Task Update(Employee employee)
    {
        await _httpClient!.PutAsync($"Employee/{employee.Id}", JsonContent.Create(employee));
    }

    public async Task Delete(Employee employee)
    {
        await _httpClient!.DeleteAsync($"Employee/{employee.Id}");
    }
}