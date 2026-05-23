using HR_Management.Domain.Entities;

namespace HR_Management.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetByDepartmentIdAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken cancellationToken = default);
    Task<int> CountByDepartmentIdAsync(int departmentId, CancellationToken cancellationToken = default);
}
