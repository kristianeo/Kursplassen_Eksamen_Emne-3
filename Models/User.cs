using Kursplassen_Eksamen_Emne_3.Dtos;

namespace Kursplassen_Eksamen_Emne_3.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Course> OwnedCourses { get; set; } = new List<Course>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public UserDto ToDto()
    {
        return new UserDto
        {
            Id = Id,
            Email = Email,
            DisplayName = DisplayName,
            Role = Role,
            CreatedAtUtc = CreatedAtUtc
        };
    }
}