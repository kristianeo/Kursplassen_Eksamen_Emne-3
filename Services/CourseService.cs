using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class CourseService(KursplassenDbContext connection) : ICourseService
{
    public async Task<IEnumerable<CourseDto>> GetAll()
    {
        var courses = await connection.Courses
            .Where(c => c.Status == CourseStatus.Published 
                        && c.StartsAtUtc >= DateTime.UtcNow)
            .ToListAsync();

        return courses.Select(c => c.ToDto()).ToList();
    }

    public async Task<IEnumerable<CourseDto>> GetUserCourses(Guid userId)
    {
        var courses = await connection.Courses
            .Where(c => c.OwnerUserId == userId)
            .Include(c => c.Owner)
            .AsNoTracking()
            .OrderBy(c => c.StartsAtUtc)
            .ToListAsync();

        return courses.Select(c => c.ToDto()).ToList();
    }
        
    public async Task<CourseDto?> GetById(Guid id)
    {
        var course = await connection.Courses.FirstOrDefaultAsync(c => c.Id == id);

        return course?.ToDto();
    }
    
    public Task<CourseDto> Create(CreateCourseDto dto)
    {
        throw new NotImplementedException();
    }

    //TODO: Add forbidden response with userid 
    public async Task<RequestResponse> Delete(Guid id)
    {
        var course = await connection.Courses.FindAsync(id);
        if (course is null) return RequestResponse.NotFound;
        
        connection.Courses.Remove(course);
        await connection.SaveChangesAsync();
        return RequestResponse.Approved;
    }

    //TODO: Needs EditCourseDTO
    public Task<CourseDto> Edit(Guid id)
    {
        throw new NotImplementedException();
    }
}