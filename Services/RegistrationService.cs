using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class RegistrationService(KursplassenDbContext connection) : IRegistrationService
{
    public async Task<IEnumerable<RegistrationDto>> GetAll()
    {
        var registrations = await connection.Registrations.ToListAsync();

        return registrations.Select(r => r.ToDto()).ToList();
    }

    public async Task<IEnumerable<RegistrationDto>> GetUserRegistrations(Guid userId)
    {
        var registrations = await connection.Registrations
                    .Where(r => r.ParticipantUserId == userId)
                    .Include(r => r.Participant)
                    .Include(r => r.Course)
                    .AsNoTracking()
                    .OrderBy(r => r.Course.StartsAtUtc)
                    .ToListAsync();

        return registrations.Select(r => r.ToDto()).ToList();
    }
        

    public async Task<RegistrationDto?> GetById(Guid id)
    {
        var registration = await connection.Registrations.FirstOrDefaultAsync(r => r.Id == id);

        return registration?.ToDto();
    }

    //TODO: CreateRegistrationDTO
    public Task<RegistrationDto> Create()
    {
        throw new NotImplementedException();
    }

    //TODO: Add forbidden response with userid 
    public async Task<RequestResponse> Delete(Guid id)
    {
        var registration = await connection.Registrations.FindAsync(id);
        if (registration == null) return RequestResponse.NotFound;

        connection.Registrations.Remove(registration);
        await connection.SaveChangesAsync();
        return RequestResponse.Approved;
    }

    //TODO: EditRegistrationDto
    public Task<RegistrationDto> Edit(Guid id)
    {
        throw new NotImplementedException();
    }
}