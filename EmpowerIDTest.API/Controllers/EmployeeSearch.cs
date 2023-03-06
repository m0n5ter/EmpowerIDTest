using System.Globalization;
using EmpowerIDTest.Shared;
using Microsoft.EntityFrameworkCore;

namespace EmpowerIDTest.API.Controllers;

public static class EmployeeSearch
{
    public static async Task<PagedList<Employee>> SearchAsync(this IQueryable<Employee> employees, EmployeeSearchRequest? request)
    {
        var searchTerm = request?.SearchTerm?.Trim();
        var sort = request?.Sort;
        var pageSize = request?.PageSize ?? 50;
        var pageNumber = request?.PageNumber ?? 1;
        var fullCount = employees.Count();

        if (searchTerm != null)
        {
            employees = employees.Where(_ =>
                (_.Email != null && _.Email.Contains(searchTerm))
                || (_.Name.Contains(searchTerm))
                || (_.Phone != null && _.Phone.Contains(searchTerm))
                || (_.DOB != null && _.DOB.Value.ToString(CultureInfo.InvariantCulture).Contains(searchTerm))
                || (_.Department != null && _.Department.Contains(searchTerm)));
        }

        employees = sort switch
        {
            "a_Name" => employees.OrderBy(_ => _.Name),
            "d_Name" => employees.OrderByDescending(_ => _.Name),
            "a_Email" => employees.OrderBy(_ => _.Email),
            "d_Email" => employees.OrderByDescending(_ => _.Email),
            "a_Phone" => employees.OrderBy(_ => _.Phone),
            "d_Phone" => employees.OrderByDescending(_ => _.Phone),
            "a_DOB" => employees.OrderBy(_ => _.DOB),
            "d_DOB" => employees.OrderByDescending(_ => _.DOB),
            "a_Department" => employees.OrderBy(_ => _.Department),
            "d_Department" => employees.OrderByDescending(_ => _.Department),
            _ => employees
        };

        return new PagedList<Employee>(await employees
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToArrayAsync(), fullCount, await employees.CountAsync(), pageSize, pageNumber);
    }
}