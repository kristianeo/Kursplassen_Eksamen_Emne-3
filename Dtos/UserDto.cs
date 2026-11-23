using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}