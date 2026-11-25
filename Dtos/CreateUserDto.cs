using System.ComponentModel.DataAnnotations;

namespace Kursplassen_Eksamen_Emne_3.Dtos;

public class CreateUserDto
{
    [Required, EmailAddress(ErrorMessage = "Not a valid email address")] 
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters long")] 
    public string Password { get; set; } = string.Empty;
    
    [Required, StringLength(120, MinimumLength = 3, 
         ErrorMessage = "Display name must be between 3 and 120 characters long")] 
    public string DisplayName { get; set; } = string.Empty;
}