using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3;

public static class DbSeeder
{
    // All seeded users share this password.
    private const string TestPassword = "Test1234!";
    
    // Fixed users Guids
    public static readonly Guid Ida   = Guid.Parse("11111111-0000-0000-0000-000000000001");
    public static readonly Guid Omar  = Guid.Parse("11111111-0000-0000-0000-000000000002");
    public static readonly Guid Nora  = Guid.Parse("11111111-0000-0000-0000-000000000003");
    public static readonly Guid Emil  = Guid.Parse("11111111-0000-0000-0000-000000000004");
    public static readonly Guid Sara  = Guid.Parse("11111111-0000-0000-0000-000000000005");
    public static readonly Guid Jonas = Guid.Parse("11111111-0000-0000-0000-000000000006");
    
    // Fixed courses Guids
    public static readonly Guid PythonCourse   = Guid.Parse("22222222-0000-0000-0000-000000000001");
    public static readonly Guid SqlCourse      = Guid.Parse("22222222-0000-0000-0000-000000000002");
    public static readonly Guid RestCourse     = Guid.Parse("22222222-0000-0000-0000-000000000003");
    public static readonly Guid GitCourse      = Guid.Parse("22222222-0000-0000-0000-000000000004");
    public static readonly Guid DockerCourse   = Guid.Parse("22222222-0000-0000-0000-000000000005");
    public static readonly Guid SecurityCourse = Guid.Parse("22222222-0000-0000-0000-000000000006");
    
    // Fixed registration Guids 
    public static readonly Guid NoraPython     = Guid.Parse("33333333-0000-0000-0000-000000000001");
    public static readonly Guid EmilPython     = Guid.Parse("33333333-0000-0000-0000-000000000002");
    public static readonly Guid NoraSql        = Guid.Parse("33333333-0000-0000-0000-000000000003");
    public static readonly Guid SaraSql        = Guid.Parse("33333333-0000-0000-0000-000000000004");
    public static readonly Guid JonasSecurity  = Guid.Parse("33333333-0000-0000-0000-000000000005");

    public static async Task SeedAsync(KursplassenDbContext db, IPasswordHasher<User> hasher)
    {
        // Only seed an empty database, so restarting the app never duplicates data
        if (await db.Users.AnyAsync()) return;

        var now = DateTime.UtcNow;

        User NewUser(Guid id, string name, string email, UserRole role)
        {
            var user = new User
            {
                Id = id,
                DisplayName = name,
                Email = email,
                Role = role,
                CreatedAtUtc = now
            };
            user.PasswordHash = hasher.HashPassword(user, TestPassword);
            return user;
        }

        var ida   = NewUser(Ida, "Ida Hansen", "ida@example.com",   UserRole.Instructor);
        var omar  = NewUser(Omar, "Omar Ali",   "omar@example.com",  UserRole.Instructor);
        var nora  = NewUser(Nora, "Nora Berg",  "nora@example.com",  UserRole.Participant);
        var emil  = NewUser(Emil, "Emil Olsen", "emil@example.com",  UserRole.Participant);
        var sara  = NewUser(Sara, "Sara Nilsen","sara@example.com",  UserRole.Participant);
        var jonas = NewUser(Jonas, "Jonas Lund", "jonas@example.com", UserRole.Participant);

        Course NewCourse(Guid id, string title, string description, User owner,
            DateTime start, DateTime end, string location, int capacity, CourseStatus status) => new()
        {
            Id = id,
            OwnerUserId = owner.Id,
            Title = title,
            Description = description,
            StartsAtUtc = start,
            EndsAtUtc = end,
            Location = location,
            Capacity = capacity,
            Status = status,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        static DateTime Utc(int y, int m, int d, int h) => new(y, m, d, h, 0, 0, DateTimeKind.Utc);

        var python = NewCourse(PythonCourse, "Grunnleggende Python", "En introduksjon til programmering med Python.",
            ida, Utc(2026, 12, 15, 9), Utc(2026, 12, 15, 14), "Sandefjord, rom A", 12, CourseStatus.Published);

        var sql = NewCourse(SqlCourse, "Introduksjon til SQL", "Lær å hente og endre data i relasjonsdatabaser.",
            omar, Utc(2026, 12, 16, 9), Utc(2026, 12, 16, 12), "Nettbasert", 2, CourseStatus.Published);

        var rest = NewCourse(RestCourse, "REST API med ASP.NET Core", "Bygg et REST API med ASP.NET Core.",
            ida, Utc(2026, 12, 17, 9), Utc(2026, 12, 17, 15), "Sandefjord, rom B", 10, CourseStatus.Draft);

        var git = NewCourse(GitCourse, "Git og GitHub", "Versjonskontroll og samarbeid med Git og GitHub.",
            omar, Utc(2026, 12, 18, 9), Utc(2026, 12, 18, 12), "Nettbasert", 8, CourseStatus.Cancelled);

        var docker = NewCourse(DockerCourse,"Docker for utviklere", "Containere og Docker i utviklingsarbeid.",
            omar, Utc(2026, 11, 10, 9), Utc(2026, 11, 10, 14), "Sandefjord, rom A", 10, CourseStatus.Published);

        var security = NewCourse(SecurityCourse, "Sikkerhet i webapplikasjoner", "Vanlige trusler og hvordan du forsvarer deg mot dem.",
            ida, Utc(2026, 12, 21, 9), Utc(2026, 12, 21, 13), "Nettbasert", 6, CourseStatus.Published);

        Registration Active(Guid id, Course course, User participant, DateTime at) => new()
        {
            Id = id,
            CourseId = course.Id,
            ParticipantUserId = participant.Id,
            RegisteredAtUtc = at,
            ActiveParticipantUserId = participant.Id
        };

        Registration Cancelled(Guid id, Course course, User participant, DateTime at, DateTime cancelledAt) => new()
        {
            Id = id,
            CourseId = course.Id,
            ParticipantUserId = participant.Id,
            RegisteredAtUtc = at,
            CancelledAtUtc = cancelledAt,
            ActiveParticipantUserId = null 
        };

        db.Users.AddRange(ida, omar, nora, emil, sara, jonas);
        db.Courses.AddRange(python, sql, rest, git, docker, security);
        db.Registrations.AddRange(
            Active(NoraPython, python, nora, Utc(2026, 11, 20, 10)),
            Active(EmilPython, python, emil, Utc(2026, 11, 20, 11)),
            Active(NoraSql, sql, nora, Utc(2026, 11, 20, 12)),
            Active(SaraSql, sql, sara, Utc(2026, 11, 20, 13)),
            Cancelled(JonasSecurity, security, jonas, Utc(2026, 11, 21, 9), Utc(2026, 11, 22, 9)));

        await db.SaveChangesAsync();
    }
}