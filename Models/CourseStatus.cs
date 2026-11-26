using System.Text.Json.Serialization;

namespace Kursplassen_Eksamen_Emne_3.Models;

[JsonConverter(typeof(JsonStringEnumConverter<CourseStatus>))]
public enum CourseStatus
{
    Draft,
    Published,
    Cancelled
}