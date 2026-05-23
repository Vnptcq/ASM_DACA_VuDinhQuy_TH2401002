using HR_Management.Application.DTOs;
using HR_Management.Application.Interfaces;
using HR_Management.Domain.Entities;
using HR_Management.Domain.Repositories;
using HR_Management.Application.Security;

namespace HR_Management.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(bool Success, string? ErrorMessage, User? User)> RegisterAsync(
        RegisterViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.EmailExistsAsync(model.Email, cancellationToken))
        {
            return (false, "An account with this email already exists.", null);
        }

        var user = new User
        {
            Email = model.Email.Trim().ToLowerInvariant(),
            FullName = model.FullName.Trim(),
            PasswordHash = PasswordHasher.Hash(model.Password)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        return (true, null, user);
    }

    public async Task<(bool Success, string? ErrorMessage, User? User)> LoginAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(model.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !PasswordHasher.Verify(model.Password, user.PasswordHash))
        {
            return (false, "Invalid email or password.", null);
        }

        return (true, null, user);
    }
}
