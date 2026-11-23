using Kursplassen_Eksamen_Emne_3.Models;

namespace Kursplassen_Eksamen_Emne_3.Services;

public interface IRegistrationService
{
    public Task<IEnumerable<Registration>> GetAll();
    public Task<IEnumerable<Registration>> GetUserRegistrations(Guid userId);
    public Task<Registration?> GetById(Guid id);
    public Task<Registration> Create();
    public Task<RequestResponse> Delete(Guid id);
    public Task<Registration> Edit(Guid id);
}