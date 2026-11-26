namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class RegistrationDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid ParticipantUserId { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
}