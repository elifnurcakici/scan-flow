using backend.Data;
using backend.Entities;
using backend.DTOs.Auth; 
using backend.Services.Interfaces;
using backend.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;

    public AuthService(AppDbContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<User?> UpdateAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return null;

        var normalizedEmail = request.Email?.Trim().ToLower();
        var isEmailChanged = !string.IsNullOrWhiteSpace(normalizedEmail) &&
            !string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase);
        var isPasswordChanged = !string.IsNullOrWhiteSpace(request.Password);

        if (isEmailChanged)
        {
            var emailExists = await _context.Users
                .AnyAsync(x => x.Id != userId && x.Email == normalizedEmail);

            if (emailExists)
                throw new BadRequestException("This email is already registered.");

            user.Email = normalizedEmail!;
        }

        if (isPasswordChanged)
        {
            user.PasswordHash = _passwordService.HashPassword(request.Password!);
        }

        if (isEmailChanged || isPasswordChanged)
        {
            user.TokenVersion += 1;
            user.RefreshTokenHash = null;
            user.RefreshTokenExpiresAt = null;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return user;
    }
}
