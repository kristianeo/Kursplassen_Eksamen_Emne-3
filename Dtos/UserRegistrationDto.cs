namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class UserRegistrationDto
{
    public Guid Id { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public CourseSummaryDto Course { get; set; } = null!;
}