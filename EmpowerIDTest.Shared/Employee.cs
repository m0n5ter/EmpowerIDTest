using System.ComponentModel.DataAnnotations;

namespace EmpowerIDTest.Shared;

public class Employee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string Password { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Email { get; set; }

    [MaxLength(15)]
    public string? Phone { get; set; }
    
    public DateTime? DOB { get; set; }

    [MaxLength(50)]
    public string? Department { get; set; }
}