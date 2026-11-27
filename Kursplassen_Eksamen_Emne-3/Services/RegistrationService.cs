using System.Data;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Logging;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class RegistrationService(KursplassenDbContext connection,
    ILogger<RegistrationService> logger) : IRegistrationService
{

    public async Task<IEnumerable<UserRegistrationDto>> GetUserRegistrations(Guid userId)
    {
        return await connection.Registrations
             .AsNoTracking()
             .Where(r => r.ParticipantUserId == userId)
             .OrderBy(r => r.Course.StartsAtUtc)
             .Select(r => new UserRegistrationDto
         {
             Id = r.Id,
             RegisteredAtUtc = r.RegisteredAtUtc,
             CancelledAtUtc = r.CancelledAtUtc,
             Course = new CourseSummaryDto
             {
                 Id = r.Course.Id,
                 Title = r.Course.Title,
                 StartsAtUtc = r.Course.StartsAtUtc,
             }
         })
         .ToListAsync();
    }

    //TODO: how to physically test with two at once 
    public async Task<RegistrationDto> Create(Guid courseId, Guid userId)
    {
        await using var tx = await connection.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var course = await connection.Courses.FindAsync(courseId)
                         ?? throw new NotFoundException("Course not found");

            if (course.Status != CourseStatus.Published)
                throw new ConflictException("Course not open for registration");

            if (course.StartsAtUtc <= DateTime.UtcNow)
                throw new ConflictException("Course has already started");

            var alreadyRegistered = await connection.Registrations
                .AnyAsync(r => r.CourseId == courseId && r.ActiveParticipantUserId == userId);
            if (alreadyRegistered)
                throw new ConflictException("You are already registered for this course");

            var activeCount = await connection.Registrations
                .CountAsync(r => r.CourseId == courseId && r.CancelledAtUtc == null);
            if (activeCount >= course.Capacity)
                throw new ConflictException("Course is at max capacity");

            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                ParticipantUserId = userId,
                RegisteredAtUtc = DateTime.UtcNow,
                ActiveParticipantUserId = userId
            };

            connection.Registrations.Add(registration);
            await connection.SaveChangesAsync();
            await tx.CommitAsync();

            Log.CourseRegistered(logger, courseId, userId);
            return registration.ToDto();
        }
        catch (Exception ex) when (ex is DbUpdateException or MySqlException { Number: 1213 })
        {
            throw new ConflictException(
                "Could not complete the registration. You may already be registered, " +
                "or the course just filled up. Please try again.");
        }
    }
    
    public async Task Cancel(Guid id)
    {
        //Do not particularly need this exception, as it is handled by the policy
        var registration = await connection.Registrations.FindAsync(id) ??
                           throw new NotFoundException("Registration not found");
        
        if (registration.CancelledAtUtc is not null)
            throw new ConflictException("The registration is already cancelled");

        registration.ActiveParticipantUserId = null;
        registration.CancelledAtUtc = DateTime.UtcNow;
        
        await connection.SaveChangesAsync();
        Log.RegistrationCancelled(logger, id);
    }
}