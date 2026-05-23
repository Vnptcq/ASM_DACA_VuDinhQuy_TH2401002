using AutoMapper;
using HR_Management.Application.Interfaces;
using HR_Management.Application.Mappings;
using HR_Management.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HR_Management.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        services.AddSingleton<IMapper>(mapperConfig.CreateMapper());

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }
}
