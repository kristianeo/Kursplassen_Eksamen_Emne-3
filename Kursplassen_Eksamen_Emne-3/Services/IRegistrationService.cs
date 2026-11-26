using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface IRegistrationService
{
    public Task<IEnumerable<UserRegistrationDto>> GetUserRegistrations(Guid userId);
    public Task<RegistrationDto> Create(Guid courseId, Guid userId);
    public Task Cancel(Guid id);
}