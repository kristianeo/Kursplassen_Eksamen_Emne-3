using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface ICourseService
{
    public Task<IEnumerable<Course>> GetAll();
    public Task<IEnumerable<Course>> GetUserCourses(Guid userId);
    public Task<Course?> GetById(Guid id);
    public Task<Course> Create(CreateCourseDto dto);
    public Task<RequestResponse> Delete(Guid id);
    public Task<Course> Edit(Guid id);
}