using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Logging;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class UserService(KursplassenDbContext connection, 
    IPasswordHasher<User> hasher,
    IConfiguration config,
    ILogger<UserService> logger) : IUserService
{
    public async Task<UserDto> RegisterAsync(CreateUserDto dto)
    {
        await using var tx = await connection.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            if (await connection.Users.AnyAsync(u => u.Email == email))
                throw new ConflictException("The email is already in use");

            User user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                DisplayName = dto.DisplayName,
                Role = UserRole.Participant,
                CreatedAtUtc = DateTime.UtcNow
            };

            user.PasswordHash = hasher.HashPassword(user, dto.Password);

            connection.Users.Add(user);
            await connection.SaveChangesAsync();
            await tx.CommitAsync();

            Log.UserRegistered(logger, user.Id);
            return user.ToDto();
        }
        catch (Exception ex) when (ex is DbUpdateException)
        {
            throw new ConflictException(
                "Could not complete the registration. You may already be registered, " +
                "and the email already in use. Please try again.");
        }
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await connection.Users.SingleOrDefaultAsync(u => u.Email.Equals(email));
        if (user is null ||
            hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
        {
            Log.LoginFailed(logger);
            throw new UnauthorizedException("Invalid email or password.");
        }

        Log.LoginSuccessful(logger, user.Id);
        return new LoginResponseDto { Token = CreateToken(user) };
    }
    
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}