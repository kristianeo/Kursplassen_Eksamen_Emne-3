using Kursplassen_Eksamen_Emne_3.Dtos;

namespace Kursplassen_Eksamen_Emne_3.Models;

public class Course
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public CourseStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public User Owner { get; set; } = null!;
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    
    public CourseDto ToDto(int registeredCount = 0)
    {
        return new CourseDto
        {
            Id = Id,
            Title = Title,
            Description = Description,
            StartsAtUtc = StartsAtUtc,
            EndsAtUtc = EndsAtUtc,
            Location = Location,
            Capacity = Capacity,
            Status = Status,
            RegisteredCount = registeredCount,
            AvailableSpots = Math.Max(0, Capacity - registeredCount)
        };
    }
}