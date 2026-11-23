using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace Kursplassen_Eksamen_Emne_3.Services;

public class UserService(KursplassenDbContext connection) : IUserService
{
    public Task<RequestResponse> RegisterAsync()
    {
        throw new NotImplementedException();
    }

    public Task<RequestResponse> LoginAsync()
    {
        throw new NotImplementedException();
    }
}