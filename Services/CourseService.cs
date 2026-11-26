using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Exceptions;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;
using ValidationException = Kursplassen_Eksamen_Emne_3.Exceptions.ValidationException;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class CourseService(KursplassenDbContext connection,
    ILogger<Course> _logger) : ICourseService
{
    public async Task<IEnumerable<CourseSummaryDto>> GetAll() =>  
        await connection.Courses
            .AsNoTracking()
            .Where(c => c.Status == CourseStatus.Published 
                        && c.StartsAtUtc >= DateTime.UtcNow)
            .Select(c => new CourseSummaryDto()
            {
                Id = c.Id,
                Title = c.Title,
                StartsAtUtc = c.StartsAtUtc
            })
            .ToListAsync();
        
    public async Task<CourseDto> GetById(Guid id)
    {
        var course = await connection.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                Course = c,
                Registered = c.Registrations.Count(r => r.CancelledAtUtc == null)
            })
            .FirstOrDefaultAsync();
        
        if (course is null)
            throw new NotFoundException("Course not found.");
        return course.Course.ToDto(course.Registered);
    }

    public async Task<CourseDto> Create(CreateCourseDto dto, Guid userId)
    {
        if (dto.StartsAtUtc <= DateTime.UtcNow)
            throw new ValidationException("Start time must be in the future");
        
        if (dto.EndsAtUtc <= dto.StartsAtUtc)
            throw new ValidationException("End time must be after start time.");
        
        Course course = new Course
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            StartsAtUtc = dto.StartsAtUtc,
            EndsAtUtc = dto.EndsAtUtc,
            Location = dto.Location,
            Capacity = dto.Capacity,
            Status = dto.Status,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        connection.Courses.Add(course);
        await connection.SaveChangesAsync();

        _logger.LogInformation(
            "Course {CourseId} created by user {UserId}", course.Id, userId);
        return course.ToDto();
    }
    
    public async Task Delete(Guid id)
    {
        var course = await connection.Courses.FindAsync(id) ?? 
                     throw new NotFoundException("Course not found");
        
        var hasActive = await connection.Registrations
            .AnyAsync(r => r.CourseId == id && r.CancelledAtUtc == null);
        if (hasActive)
            throw new ConflictException("A course with active registrations can't be deleted.");
        
        connection.Courses.Remove(course);
        await connection.SaveChangesAsync();
        _logger.LogInformation(
            "Course {CourseId} deleted by user {UserId}", course.Id, course.OwnerUserId);
    }
    
    public async Task<CourseDto> Edit(EditCourseDto dto, Guid id)
    {
        var course = await connection.Courses.FindAsync(id);
        if (course == null)
            throw new NotFoundException("Course not found");
        
        if (dto.EndsAtUtc <= dto.StartsAtUtc)
            throw new ValidationException("End time must be after start time.");
        
        var activeCount = await connection.Registrations
            .CountAsync(r => r.CourseId == id && r.CancelledAtUtc == null);
        if (dto.Capacity < activeCount)
            throw new ConflictException("Capacity can't be lower than the number of active registrations.");

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.StartsAtUtc = dto.StartsAtUtc;
        course.EndsAtUtc = dto.EndsAtUtc;
        course.Location = dto.Location;
        course.Capacity = dto.Capacity;
        course.Status = dto.Status;
        course.UpdatedAtUtc = DateTime.UtcNow;
        
        await connection.SaveChangesAsync();

        _logger.LogInformation(
            "Course {CourseId} edited by user {UserId}", course.Id, course.OwnerUserId);
        return course.ToDto(activeCount);
    }
    
    
}