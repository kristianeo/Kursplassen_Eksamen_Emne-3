using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class CourseService(KursplassenDbContext connection) : ICourseService
{
    public async Task<IEnumerable<Course>> GetAll()
    {
        return await connection.Courses.ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetUserCourses(Guid userId) =>
        await connection.Courses
            .Where(c => c.OwnerUserId == userId)
            .Include(c => c.Owner)
            .AsNoTracking()
            .OrderBy(c => c.StartsAtUtc)
            .ToListAsync();
    public async Task<Course?> GetById(Guid id)
    {
        return await connection.Courses.FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public Task<Course> Create(CreateCourseDto dto)
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
    public Task<Course> Edit(Guid id)
    {
        throw new NotImplementedException();
    }
}