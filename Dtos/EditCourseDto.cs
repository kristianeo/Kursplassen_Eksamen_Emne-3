using System.ComponentModel.DataAnnotations;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class EditCourseDto
{
    [StringLength(180), MinLength(3)]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    [StringLength(180)] public string Location { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int Capacity { get; set; }
    public CourseStatus Status { get; set; }
}