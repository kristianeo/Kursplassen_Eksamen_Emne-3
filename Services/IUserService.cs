using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface IUserService
{
    Task<RequestResponse> RegisterAsync();
    Task<RequestResponse> LoginAsync();
    
}