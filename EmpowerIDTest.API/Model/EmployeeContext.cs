using EmpowerIDTest.Shared;
using Microsoft.EntityFrameworkCore;

namespace EmpowerIDTest.API.Model;

public class EmployeeContext : DbContext
{
    protected EmployeeContext()
    {
    }

    public EmployeeContext(DbContextOptions<EmployeeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; } = null!;
}