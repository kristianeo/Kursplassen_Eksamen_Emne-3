using Kursplassen_Eksamen_Emne_3;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Models;
using Kursplassen_Eksamen_Emne_3.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Kursplassen.Tests.Unit;

[Trait("Category", "Unit")]
public class AuthServiceTests
{
    private static KursplassenDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<KursplassenDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new KursplassenDbContext(options);
    }
    
    // Stub: a real configuration with test values, used by CreateToken
    private static IConfiguration CreateConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-key-that-is-at-least-32-characters-long",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            })
            .Build();
    
    // Mock: decides what VerifyHashedPassword returns
    private static Mock<IPasswordHasher<User>> CreateHasher(PasswordVerificationResult result)
    {
        var hasher = new Mock<IPasswordHasher<User>>();
        hasher.Setup(h => h.VerifyHashedPassword
                (It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(result);
        return hasher;
    }
    
    private static async Task<User> AddUserAsync(KursplassenDbContext db)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "nora@example.com",
            PasswordHash = "irrelevant-because-the-hasher-is-mocked",
            Role = UserRole.Participant
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task Login_InvalidEmail_ThrowsUnauthorized()
    {
        await using var db = CreateDb();
        
        var hasher = CreateHasher(PasswordVerificationResult.Success);
        var service = new UserService(db, hasher.Object, CreateConfig(), NullLogger<UserService>.Instance);
        
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(new LoginDto { Email = "nobody@example.com", Password = "Test1234!" }));
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsUnauthorized()
    {
        await using var db = CreateDb();
        await AddUserAsync(db);
        
        var hasher = CreateHasher(PasswordVerificationResult.Failed);
        var service = new UserService(db, hasher.Object, CreateConfig(), NullLogger<UserService>.Instance);
        
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(new LoginDto { Email = "nora@example.com", Password = "Test1234!" }));
    }

    [Fact]
    public async Task Login_SuccessfulLogin_ReturnsToken()
    {
        await using var db = CreateDb();
        await AddUserAsync(db);
        
        var hasher = CreateHasher(PasswordVerificationResult.Success);
        var service = new UserService(db, hasher.Object, CreateConfig(), NullLogger<UserService>.Instance);

        var loginResponseDto = await service.LoginAsync(new LoginDto { Email = "nora@example.com", Password = "Test1234!" });
        
        Assert.NotNull(loginResponseDto.Token);
        hasher.Verify(h => h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), "Test1234!"), Times.Once);
    }
    
    [Fact]
    public async Task Login_EmailIgnoresCaseAndSurroundingSpace_ReturnsDto()
    {
        await using var db = CreateDb();
        await AddUserAsync(db);
        
        var hasher = CreateHasher(PasswordVerificationResult.Success);
        var service = new UserService(db, hasher.Object, CreateConfig(), NullLogger<UserService>.Instance);

        var loginResponseDto = await service.LoginAsync(new LoginDto { Email = " noRa@eXAmple.cOm   ", Password = "Test1234!" });
        
        Assert.NotNull(loginResponseDto);
    }
}