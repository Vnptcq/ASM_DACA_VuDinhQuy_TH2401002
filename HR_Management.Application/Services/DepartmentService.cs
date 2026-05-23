using AutoMapper;
using HR_Management.Application.DTOs;
using HR_Management.Application.Interfaces;
using HR_Management.Domain.Entities;
using HR_Management.Domain.Repositories;

namespace HR_Management.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DepartmentDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _departmentRepository.RefreshEmployeeCountsAsync(cancellationToken);
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DepartmentDTO>>(departments);
    }

    public async Task<DepartmentDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return null;
        }

        department.NumberOfPersonals = await _employeeRepository.CountByDepartmentIdAsync(id, cancellationToken);
        return _mapper.Map<DepartmentDTO>(department);
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
        DepartmentDTO dto,
        CancellationToken cancellationToken = default)
    {
        if (await _departmentRepository.DepartmentCodeExistsAsync(dto.DepartmentCode, cancellationToken: cancellationToken))
        {
            return (false, "Department code already exists.");
        }

        var department = _mapper.Map<Department>(dto);
        department.NumberOfPersonals = 0;
        await _departmentRepository.AddAsync(department, cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
        DepartmentDTO dto,
        CancellationToken cancellationToken = default)
    {
        var existing = await _departmentRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (existing is null)
        {
            return (false, "Department not found.");
        }

        if (await _departmentRepository.DepartmentCodeExistsAsync(dto.DepartmentCode, dto.Id, cancellationToken))
        {
            return (false, "Department code already exists.");
        }

        existing.DepartmentName = dto.DepartmentName;
        existing.DepartmentCode = dto.DepartmentCode;
        existing.Location = dto.Location;
        existing.NumberOfPersonals = await _employeeRepository.CountByDepartmentIdAsync(dto.Id, cancellationToken);

        await _departmentRepository.UpdateAsync(existing, cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (!await _departmentRepository.ExistsAsync(id, cancellationToken))
        {
            return (false, "Department not found.");
        }

        var employeeCount = await _employeeRepository.CountByDepartmentIdAsync(id, cancellationToken);
        if (employeeCount > 0)
        {
            return (false, "Cannot delete a department that has employees assigned.");
        }

        await _departmentRepository.DeleteAsync(id, cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyList<DepartmentStatisticsDTO>> GetStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        var statistics = new List<DepartmentStatisticsDTO>();

        foreach (var department in departments)
        {
            var count = await _employeeRepository.CountByDepartmentIdAsync(department.Id, cancellationToken);
            statistics.Add(new DepartmentStatisticsDTO
            {
                DepartmentId = department.Id,
                DepartmentName = department.DepartmentName,
                DepartmentCode = department.DepartmentCode,
                Location = department.Location,
                TotalEmployees = count
            });

            department.NumberOfPersonals = count;
            await _departmentRepository.UpdateAsync(department, cancellationToken);
        }

        return statistics;
    }
}
