using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Application.Abstractions.Security;
using AiPoweredCodex.Application.DTOs.Auth;
using AiPoweredCodex.Domain.Entities;
using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Application.Services;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (request.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Admin registration is not allowed through the public API.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var user = new AppUser(request.FullName, request.Email, _passwordHasher.Hash(request.Password), request.Role);
        await _userRepository.AddAsync(user, cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new InvalidOperationException("Invalid email or password.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(AppUser user)
    {
        var token = _tokenService.GenerateToken(user);
        return new AuthResponse(token.AccessToken, token.ExpiresAtUtc, new UserDto(user.Id, user.FullName, user.Email, user.Role.ToString()));
    }
}
