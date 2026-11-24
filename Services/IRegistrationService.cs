using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface IRegistrationService
{
    public Task<IEnumerable<RegistrationDto>> GetAll();
    public Task<IEnumerable<RegistrationDto>> GetUserRegistrations(Guid userId);
    public Task<RegistrationDto?> GetById(Guid id);
    public Task<RegistrationDto> Create();
    public Task<RequestResponse> Delete(Guid id);
    public Task<RegistrationDto> Edit(Guid id);
}