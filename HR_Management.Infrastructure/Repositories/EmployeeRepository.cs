using HR_Management.Domain.Entities;
using HR_Management.Domain.Repositories;
using HR_Management.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .OrderBy(e => e.EmployeeName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetByDepartmentIdAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.DepartmentId == departmentId)
            .OrderBy(e => e.EmployeeName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.AnyAsync(e => e.EmployeeCode == employeeCode, cancellationToken);
    }

    public async Task<int> CountByDepartmentIdAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.CountAsync(e => e.DepartmentId == departmentId, cancellationToken);
    }
}
