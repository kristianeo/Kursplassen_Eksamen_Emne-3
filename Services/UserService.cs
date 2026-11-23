using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class UserService(KursplassenDbContext connection, 
    IPasswordHasher<User> hasher,
    IConfiguration config) : IUserService
{
    public async Task<UserDto> RegisterAsync(CreateUserDto dto)
    {
        if (await connection.Users.AnyAsync(u => u.Email == dto.Email))
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

        return user.ToDto();
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await connection.Users.SingleOrDefaultAsync(u => u.Email.Equals(dto.Email.Trim(), StringComparison.InvariantCultureIgnoreCase));
        if (user is null ||
            hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password.");

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