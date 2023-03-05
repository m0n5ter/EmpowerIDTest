using EmpowerIDTest.API.Model;
using EmpowerIDTest.Shared;
using Microsoft.EntityFrameworkCore;

namespace EmpowerIDTest.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<EmployeeContext>(opt => opt.UseInMemoryDatabase("Employees"));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        CreateDbIfNotExists(app).Wait();

        app.Run();
    }

    private static async Task CreateDbIfNotExists(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeContext>();
        await dbContext.Database.EnsureCreatedAsync();

        if (!dbContext.Employees.Any())
        {
            dbContext.Employees.AddRange(Enumerable.Range(0, 1000).Select(i =>
            {
                var name = $"User_{i:0000}";

                return new Employee
                {
                    Name = name,
                    Password = Random.Shared.Next(111111, 999999).ToString(),
                    Email = $"{name}@universe.com",
                    DOB = DateTime.Today.AddDays(-Random.Shared.Next(7000, 10000)),
                    Phone = $"({Random.Shared.Next(500,599)}) {Random.Shared.Next(1111111, 9999999)}"
                };
            }));

            await dbContext.SaveChangesAsync();
        }
    }
}