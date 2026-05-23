using HR_Management.Domain.Entities;
using HR_Management.Domain.Repositories;
using HR_Management.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.DepartmentName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        await _context.Departments.AddAsync(department, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return department;
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments.FindAsync([id], cancellationToken);
        if (department is null)
        {
            return;
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<bool> DepartmentCodeExistsAsync(string departmentCode, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Departments.Where(d => d.DepartmentCode == departmentCode);
        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task RefreshEmployeeCountsAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _context.Departments.ToListAsync(cancellationToken);
        foreach (var department in departments)
        {
            department.NumberOfPersonals = await _context.Employees
                .CountAsync(e => e.DepartmentId == department.Id, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
