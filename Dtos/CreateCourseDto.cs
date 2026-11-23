using System.ComponentModel.DataAnnotations;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class CreateCourseDto
{
    [Required]
    [StringLength(180), MinLength(3)]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required] public DateTime StartsAtUtc { get; set; }
    [Required] public DateTime EndsAtUtc { get; set; }
    [Required] 
    [StringLength(180)] public string Location { get; set; } = string.Empty;
    [Required] public int Capacity { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
}