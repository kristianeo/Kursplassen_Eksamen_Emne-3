using System.ComponentModel.DataAnnotations;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class EditCourseDto
{
    [Required, StringLength(180, MinimumLength = 3, 
         ErrorMessage = "Title must be between 3 and 180 characters long")]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    
    [StringLength(180, MinimumLength = 3, 
         ErrorMessage = "Location must be between 3 and 180 characters long")]  
    public string Location { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")] 
    public int Capacity { get; set; }
    
    public CourseStatus Status { get; set; }
}