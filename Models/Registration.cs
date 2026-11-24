using Kursplassen_Eksamen_Emne_3.Dtos;

namespace Kursplassen_Eksamen_Emne_3.Models;

public class Registration
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid ParticipantUserId { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public Guid? ActiveParticipantUserId { get; set; }
    
    public Course Course { get; set; } = null!;
    public User Participant { get; set; } = null!;
    
    public RegistrationDto ToDto()
    {
        return new RegistrationDto
        {
            Id = Id,
            CourseId = CourseId,
            ParticipantUserId = ParticipantUserId,
            RegisteredAtUtc = RegisteredAtUtc,
            CancelledAtUtc = CancelledAtUtc
        };
    }
}