using System.ComponentModel.DataAnnotations;

namespace HR_Management.Application.DTOs;

public class DepartmentDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Department name is required.")]
    [StringLength(200)]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department code is required.")]
    [StringLength(50)]
    [Display(Name = "Department Code")]
    public string DepartmentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Number of Employees")]
    public int NumberOfPersonals { get; set; }
}
