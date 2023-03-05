namespace EmpowerIDTest.Shared;

public class EmployeeSearchRequest
{
    public string? SearchTerm { get; set; }

    public string? Sort { get; set; }

    public int PageSize { get; set; } = 50;

    public int PageNumber { get; set; } = 1;
}