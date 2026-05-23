using AutoMapper;
using HR_Management.Application.DTOs;
using HR_Management.Application.Interfaces;
using HR_Management.Domain.Entities;
using HR_Management.Domain.Repositories;

namespace HR_Management.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeDTO>>(employees);
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
        EmployeeDTO dto,
        CancellationToken cancellationToken = default)
    {
        if (!await _departmentRepository.ExistsAsync(dto.DepartmentId, cancellationToken))
        {
            return (false, "Selected department does not exist.");
        }

        if (await _employeeRepository.EmployeeCodeExistsAsync(dto.EmployeeCode, cancellationToken))
        {
            return (false, "Employee code already exists.");
        }

        var employee = _mapper.Map<Employee>(dto);
        await _employeeRepository.AddAsync(employee, cancellationToken);

        var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId, cancellationToken);
        if (department is not null)
        {
            department.NumberOfPersonals = await _employeeRepository.CountByDepartmentIdAsync(dto.DepartmentId, cancellationToken);
            await _departmentRepository.UpdateAsync(department, cancellationToken);
        }

        return (true, null);
    }
}
