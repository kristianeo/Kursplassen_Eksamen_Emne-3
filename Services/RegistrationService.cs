using System.Data;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class RegistrationService(KursplassenDbContext connection) : IRegistrationService
{
    public async Task<IEnumerable<RegistrationDto>> GetAll()
    {
        var registrations = await connection.Registrations.ToListAsync();

        return registrations.Select(r => r.ToDto()).ToList();
    }

    public async Task<IEnumerable<RegistrationDto>> GetUserRegistrations(Guid userId)
    {
        var registrations = await connection.Registrations
                    .Where(r => r.ParticipantUserId == userId)
                    .Include(r => r.Participant)
                    .Include(r => r.Course)
                    .AsNoTracking()
                    .OrderBy(r => r.Course.StartsAtUtc)
                    .ToListAsync();

        return registrations.Select(r => r.ToDto()).ToList();
    }
        

    public async Task<RegistrationDto?> GetById(Guid id)
    {
        var registration = await connection.Registrations.FirstOrDefaultAsync(r => r.Id == id);

        return registration?.ToDto();
    }

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

            return registration.ToDto();
        }
        catch (Exception ex) when (ex is DbUpdateException or MySqlException { Number: 1213 })
        {
            throw new ConflictException(
                "Could not complete the registration. You may already be registered, " +
                "or the course just filled up. Please try again.");
        }
    }
    
    //TODO: Should not delete, just cancel
    public async Task Delete(Guid id)
    {
        var registration = await connection.Registrations.FindAsync(id) ??
                           throw new NotFoundException("Registration not found");

        connection.Registrations.Remove(registration);
        await connection.SaveChangesAsync();
    }

    //TODO: EditRegistrationDto
    public Task<RegistrationDto> Edit(Guid id)
    {
        throw new NotImplementedException();
    }
}