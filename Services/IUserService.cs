using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface IUserService
{
    Task<UserDto> RegisterAsync(CreateUserDto dto);
    Task<UserDto> LoginAsync(LoginDto dto);
    
}