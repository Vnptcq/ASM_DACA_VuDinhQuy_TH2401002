using HR_Management.Application.DTOs;
using HR_Management.Domain.Entities;

namespace HR_Management.Application.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string? ErrorMessage, User? User)> RegisterAsync(RegisterViewModel model, CancellationToken cancellationToken = default);
    Task<(bool Success, string? ErrorMessage, User? User)> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default);
}
