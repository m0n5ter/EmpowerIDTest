using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmpowerIDTest.API.Model;
using EmpowerIDTest.Shared;

namespace EmpowerIDTest.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly EmployeeContext _context;

    public EmployeeController(EmployeeContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Route("Search")]
    public async Task<IActionResult> Search([FromBody] EmployeeSearchRequest? request)
    {
        try
        {
            var searchTerm = request?.SearchTerm?.Trim();
            var sort = request?.Sort;
            var pageSize = request?.PageSize ?? 50;
            var pageNumber = request?.PageNumber ?? 1;

            var users = (IQueryable<Employee>) _context.Employees;

            if (searchTerm != null)
            {
                users = users.Where(_ =>
                    (_.Email != null && _.Email.Contains(searchTerm))
                    || (_.Name.Contains(searchTerm))
                    || (_.Phone != null && _.Phone.Contains(searchTerm))
                    || (_.DOB != null && _.DOB.Value.ToString(CultureInfo.InvariantCulture).Contains(searchTerm))
                    || (_.Department != null && _.Department.Contains(searchTerm)));
            }

            users = sort switch
            {
                "a_Name" => users.OrderBy(_ => _.Name),
                "d_Name" => users.OrderByDescending(_ => _.Name),
                "a_Email" => users.OrderBy(_ => _.Email),
                "d_Email" => users.OrderByDescending(_ => _.Email),
                "a_Phone" => users.OrderBy(_ => _.Phone),
                "d_Phone" => users.OrderByDescending(_ => _.Phone),
                "a_DOB" => users.OrderBy(_ => _.DOB),
                "d_DOB" => users.OrderByDescending(_ => _.DOB),
                "a_Department" => users.OrderBy(_ => _.Department),
                "d_Department" => users.OrderByDescending(_ => _.Department),
                _ => users
            };

            return Ok(new
            {
                fullCount = await _context.Employees.CountAsync(),
                filteredCount = await users.CountAsync(),
                result = await users
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToArrayAsync()
            });
        }
        catch (Exception exception)
        {
            return Problem(exception.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetUser(int id)
    {
        var user = await _context.Employees.FindAsync(id);
        return user == null ? NotFound() : user;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, Employee employee)
    {
        if (id != employee.Id) return BadRequest();

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> PostUser(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new {id = employee.Id}, employee);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Employees.FindAsync(id);
        if (user == null) return NotFound();

        _context.Employees.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id) => _context.Employees.Any(e => e.Id == id);
}