using System.ComponentModel.DataAnnotations;

namespace HR_Management.Application.DTOs;

public class EmployeeDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Employee name is required.")]
    [StringLength(200)]
    [Display(Name = "Employee Name")]
    public string EmployeeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Employee code is required.")]
    [StringLength(50)]
    [Display(Name = "Employee Code")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Display(Name = "Department")]
    public string? DepartmentName { get; set; }

    [Required(ErrorMessage = "Rank is required.")]
    [StringLength(100)]
    public string Rank { get; set; } = string.Empty;
}
