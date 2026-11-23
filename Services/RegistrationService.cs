using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class RegistrationService(KursplassenDbContext connection) : IRegistrationService
{
    public async Task<IEnumerable<Registration>> GetAll()
    {
        return await connection.Registrations.ToListAsync();
    }

    public async Task<IEnumerable<Registration>> GetUserRegistrations(Guid userId) =>
        await connection.Registrations
            .Where(r => r.ParticipantUserId == userId)
            .Include(r => r.Participant)
            .Include(r => r.Course)
            .AsNoTracking()
            .OrderBy(r => r.Course.StartsAtUtc)
            .ToListAsync();

    public async Task<Registration?> GetById(Guid id)
    {
        return await connection.Registrations.FirstOrDefaultAsync(r => r.Id == id);
    }

    //TODO: CreateRegistrationDTO
    public Task<Registration> Create()
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
    public Task<Registration> Edit(Guid id)
    {
        throw new NotImplementedException();
    }
}