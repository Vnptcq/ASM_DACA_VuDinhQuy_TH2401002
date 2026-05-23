using HR_Management.Application.DTOs;

namespace HR_Management.Application.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DepartmentDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(bool Success, string? ErrorMessage)> CreateAsync(DepartmentDTO dto, CancellationToken cancellationToken = default);
    Task<(bool Success, string? ErrorMessage)> UpdateAsync(DepartmentDTO dto, CancellationToken cancellationToken = default);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DepartmentStatisticsDTO>> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
