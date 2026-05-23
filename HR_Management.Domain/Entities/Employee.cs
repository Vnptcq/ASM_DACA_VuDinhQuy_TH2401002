namespace HR_Management.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Rank { get; set; } = string.Empty;

    public Department? Department { get; set; }
}
