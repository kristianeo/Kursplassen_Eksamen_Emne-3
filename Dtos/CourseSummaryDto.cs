namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class CourseSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
}