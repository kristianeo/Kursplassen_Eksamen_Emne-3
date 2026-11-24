using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface ICourseService
{
    public Task<IEnumerable<CourseDto>> GetAll();
    public Task<IEnumerable<CourseDto>> GetUserCourses(Guid userId);
    public Task<CourseDto?> GetById(Guid id);
    public Task<CourseDto> Create(CreateCourseDto dto);
    public Task<RequestResponse> Delete(Guid id);
    public Task<CourseDto> Edit(Guid id);
}