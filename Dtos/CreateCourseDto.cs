using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class CreateCourseDto
{
    [Required, StringLength(180, MinimumLength = 3, 
         ErrorMessage = "Title must be between 3 and 180 characters long")]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    [Required] public DateTime StartsAtUtc { get; set; }
    [Required] public DateTime EndsAtUtc { get; set; }
    
    [Required, StringLength(180, MinimumLength = 3, 
         ErrorMessage = "Location must be between 3 and 180 characters long")] 
    public string Location { get; set; } = string.Empty;
    
    [Required, Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")] 
    public int Capacity { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
}