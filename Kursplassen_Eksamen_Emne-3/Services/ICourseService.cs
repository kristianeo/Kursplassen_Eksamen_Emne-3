using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface ICourseService
{
    public Task<IEnumerable<CourseSummaryDto>> GetAll();
    public Task<CourseDto> GetById(Guid id);
    public Task<CourseDto> Create(CreateCourseDto dto, Guid userId);
    public Task Delete(Guid id);
    public Task<CourseDto> Edit(EditCourseDto dto, Guid id);
}