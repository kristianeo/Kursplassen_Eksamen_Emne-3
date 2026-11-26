using Kursplassen_Eksamen_Emne_3;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Models;
using Kursplassen_Eksamen_Emne_3.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kursplassen.Tests;

public class CourseServiceTests
{
    private static KursplassenDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<KursplassenDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new KursplassenDbContext(options);
    }
    
    private static async Task<Course> AddCourseAsync(
        KursplassenDbContext db, int capacity, int activeRegistrations)
    {
        var course = new Course { Id = Guid.NewGuid(), Capacity = capacity };
        for (var i = 0; i < activeRegistrations; i++)
        {
            var participantId = Guid.NewGuid();
            course.Registrations.Add(new Registration
            {
                Id = Guid.NewGuid(),
                ParticipantUserId = participantId,
                ActiveParticipantUserId = participantId
            });
        }
        db.Courses.Add(course);
        await db.SaveChangesAsync();
        return course;
    }

    [Fact]
    public async Task Edit_CapacityBelowActiveRegistrations_ThrowsConflict()
    {
        // Arrange
        await using var db = CreateDb();

        var course = await AddCourseAsync(db, 10, 5);

        var service = new CourseService(db, NullLogger<CourseService>.Instance);

        var editDto = new EditCourseDto
        {
            Capacity = 3,
            StartsAtUtc = DateTime.UtcNow.AddDays(2),
            EndsAtUtc = DateTime.UtcNow.AddDays(3)
        };

        // Act and assert
        await Assert.ThrowsAsync<ConflictException>(() => service.Edit(editDto, course.Id));
    }

    [Fact]
    public async Task DeleteCourseWithActiveRegistrations_ThrowsConflict()
    {
        await using var db = CreateDb();

        var course = await AddCourseAsync(db, 10, 5);

        var service = new CourseService(db, NullLogger<CourseService>.Instance);

        await Assert.ThrowsAsync<ConflictException>(() => service.Delete(course.Id));
    }
}