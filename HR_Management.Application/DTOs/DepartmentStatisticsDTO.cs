namespace HR_Management.Application.DTOs;

public class DepartmentStatisticsDTO
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int TotalEmployees { get; set; }
}
