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
            var list = await _context.Employees.SearchAsync(request);

            return Ok(new
            {
                fullCount = list.FullCount,
                filteredCount = list.FilteredCount,
                result = list
            });
        }
        catch (Exception exception)
        {
            return Problem(exception.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> Get(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        return employee == null ? NotFound() : employee;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Employee employee)
    {
        if (id != employee.Id) return BadRequest();

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> Create(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new {id = employee.Id}, employee);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool Exists(int id) => _context.Employees.Any(e => e.Id == id);
}