using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Auth;
using backend.Entities;
using backend.Services;
using backend.Services.Interfaces;
using backend.Exceptions;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly JwtService _jwtService;
    private readonly IRedisTokenBlacklistService _redisTokenBlacklistService;
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AppDbContext context,
        IPasswordService passwordService,
        JwtService jwtService,
        IRedisTokenBlacklistService redisTokenBlacklistService,
        AuthService authService,
        ILogger<AuthController> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _redisTokenBlacklistService = redisTokenBlacklistService;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (existingUser is not null)
        {
            throw new BadRequestException("This email is already registered.");
        }

        var user = new User
        {
            Email = email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            TokenVersion = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            HttpContext.TraceIdentifier,
            "User registered successfully."
        ));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            throw new UnauthorizedException("Email or password is incorrect." );
        }

        var isPasswordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Email or password is incorrect." );
        }

        var tokens = await IssueTokensAsync(user);

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(
            new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                Email = user.Email,
                AccessTokenExpiresAt = tokens.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt
            },
            HttpContext.TraceIdentifier
    ));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user is null || string.IsNullOrWhiteSpace(user.RefreshTokenHash) || user.RefreshTokenExpiresAt is null)
        {
            throw new UnauthorizedException("Refresh token is invalid.");
        }

        var incomingTokenHash = _jwtService.HashToken(request.RefreshToken);
        var isRefreshTokenValid =
            user.RefreshTokenHash == incomingTokenHash &&
            user.RefreshTokenExpiresAt > DateTime.UtcNow;

        if (!isRefreshTokenValid)
        {
            throw new UnauthorizedException("Refresh token is invalid.");
        }

        var tokens = await IssueTokensAsync(user);

        return Ok(ApiResponse<AuthResponse>.SuccessResponse(
            new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                Email = user.Email,
                AccessTokenExpiresAt = tokens.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt
            },
            HttpContext.TraceIdentifier
        ));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        var userId = GetUserId();

        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var expClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (!string.IsNullOrWhiteSpace(jti) &&
            long.TryParse(expClaim, out var expUnix))
        {
            var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            await _redisTokenBlacklistService.BlacklistAccessTokenAsync(jti, expiresAtUtc);
        }

        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAt = null;
        user.TokenVersion += 1;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            HttpContext.TraceIdentifier,
            "Logged out successfully."
        ));
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateUserRequest request)
    {
        var userId = GetUserId();
        var user = await _authService.UpdateAsync(userId, request);

        if (user is null)
            throw new NotFoundException("User not found.");

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                id = user.Id,
                email = user.Email,
                updatedAt = user.UpdatedAt
            },
            HttpContext.TraceIdentifier
        ));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = GetUserId();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                id = user.Id,
                email = user.Email,
                createdAt = user.CreatedAt,
                updatedAt = user.UpdatedAt
            },
            HttpContext.TraceIdentifier
        ));
    }

    private async Task<TokenPair> IssueTokensAsync(User user)
    {
        var tokens = _jwtService.GenerateTokens(user);

        user.RefreshTokenHash = _jwtService.HashToken(tokens.RefreshToken);
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return tokens;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(claim, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }
}
