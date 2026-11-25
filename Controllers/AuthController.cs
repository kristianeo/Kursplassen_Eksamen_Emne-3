using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;
using Kursplassen_Eksamen_Emne_3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kursplassen_Eksamen_Emne_3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(CreateUserDto dto)
    {
        return StatusCode(StatusCodes.Status201Created, 
            await service.RegisterAsync(dto));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        return await service.LoginAsync(dto);
    }

}