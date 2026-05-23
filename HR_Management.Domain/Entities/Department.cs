namespace HR_Management.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int NumberOfPersonals { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
