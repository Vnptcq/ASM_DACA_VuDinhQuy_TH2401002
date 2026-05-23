using AutoMapper;
using HR_Management.Application.DTOs;
using HR_Management.Domain.Entities;

namespace HR_Management.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Department, DepartmentDTO>();
        CreateMap<DepartmentDTO, Department>()
            .ForMember(dest => dest.Employees, opt => opt.Ignore());

        CreateMap<Employee, EmployeeDTO>()
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null));
        CreateMap<EmployeeDTO, Employee>()
            .ForMember(dest => dest.Department, opt => opt.Ignore());
    }
}
