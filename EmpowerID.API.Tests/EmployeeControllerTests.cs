using EmpowerIDTest.API.Controllers;
using EmpowerIDTest.API.Model;
using Microsoft.EntityFrameworkCore;
using EmpowerIDTest.Shared;
using Moq;

namespace EmpowerID.API.Tests;

[TestClass]
public class EmployeeControllerTests
{
    [TestMethod]
    public async Task CreateTest()
    {
        var mockSet = new Mock<DbSet<Employee>>();

        var mockContext = new Mock<EmployeeContext>();
        mockContext.Setup(m => m.Employees).Returns(mockSet.Object);

        var c = new EmployeeController(mockContext.Object);
        await c.Create(new Employee {Name = "TestEmployee", Password = "123", Email = "asd@sdf.ff"});

        mockSet.Verify(m => m.Add(It.IsAny<Employee>()), Times.Once());
    }
}