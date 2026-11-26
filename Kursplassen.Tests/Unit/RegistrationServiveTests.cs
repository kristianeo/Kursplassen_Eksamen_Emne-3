using Kursplassen_Eksamen_Emne_3;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Models;
using Kursplassen_Eksamen_Emne_3.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kursplassen.Tests;

public class RegistrationServiceTests
{
    private static KursplassenDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<KursplassenDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new KursplassenDbContext(options);
    }

    [Fact]
    public async Task Cancel_AlreadyCancelledRegistration_ThrowsConflict()
    {
        // Arrange
        await using var db = CreateDb();
        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            ParticipantUserId = Guid.NewGuid(),
            RegisteredAtUtc = DateTime.UtcNow.AddDays(-2),
            CancelledAtUtc = DateTime.UtcNow.AddDays(-1),
            ActiveParticipantUserId = null
        };
        db.Registrations.Add(registration);
        await db.SaveChangesAsync();

        var service = new RegistrationService(db, NullLogger<RegistrationService>.Instance);

        // Act and assert
        await Assert.ThrowsAsync<ConflictException>(() => service.Cancel(registration.Id));
    }

    [Fact]
    public async Task Cancel_ActiveRegistration_FreesTheSpot()
    {
        await using var db = CreateDb();
        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            ParticipantUserId = Guid.NewGuid(),
            RegisteredAtUtc = DateTime.UtcNow.AddDays(-2)
        };
        registration.ActiveParticipantUserId = registration.ParticipantUserId;
        db.Registrations.Add(registration);
        await db.SaveChangesAsync();
        
        var service = new RegistrationService(db, NullLogger<RegistrationService>.Instance);

        await service.Cancel(registration.Id);

        var saved = await db.Registrations.AsNoTracking().SingleAsync(r => r.Id == registration.Id);
        Assert.Null(saved.ActiveParticipantUserId);
        Assert.NotNull(saved.CancelledAtUtc);
    }
}