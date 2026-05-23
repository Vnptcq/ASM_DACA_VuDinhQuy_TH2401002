using HR_Management.Domain.Entities;

namespace HR_Management.Domain.Repositories;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default);
    Task UpdateAsync(Department department, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DepartmentCodeExistsAsync(string departmentCode, int? excludeId = null, CancellationToken cancellationToken = default);
    Task RefreshEmployeeCountsAsync(CancellationToken cancellationToken = default);
}
