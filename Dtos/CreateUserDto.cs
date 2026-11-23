using System.ComponentModel.DataAnnotations;

namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class CreateUserDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] [MinLength(8)] public string Password { get; set; } = string.Empty;
    [Required] [StringLength(120)] public string DisplayName { get; set; } = string.Empty;
}