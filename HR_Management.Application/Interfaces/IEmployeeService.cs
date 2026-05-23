using HR_Management.Application.DTOs;

namespace HR_Management.Application.Interfaces;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(bool Success, string? ErrorMessage)> CreateAsync(EmployeeDTO dto, CancellationToken cancellationToken = default);
}
