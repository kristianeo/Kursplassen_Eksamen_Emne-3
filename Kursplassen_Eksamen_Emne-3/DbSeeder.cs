using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3;

public static class DbSeeder
{
    // All seeded users share this password. Document it in the README.
    private const string TestPassword = "Test1234!";

    public static async Task SeedAsync(KursplassenDbContext db, IPasswordHasher<User> hasher)
    {
        // Only seed an empty database, so restarting the app never duplicates data
        if (await db.Users.AnyAsync()) return;

        var now = DateTime.UtcNow;

        User NewUser(string name, string email, UserRole role)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = name,
                Email = email,
                Role = role,
                CreatedAtUtc = now
            };
            user.PasswordHash = hasher.HashPassword(user, TestPassword);
            return user;
        }

        var ida   = NewUser("Ida Hansen", "ida@example.com",   UserRole.Instructor);
        var omar  = NewUser("Omar Ali",   "omar@example.com",  UserRole.Instructor);
        var nora  = NewUser("Nora Berg",  "nora@example.com",  UserRole.Participant);
        var emil  = NewUser("Emil Olsen", "emil@example.com",  UserRole.Participant);
        var sara  = NewUser("Sara Nilsen","sara@example.com",  UserRole.Participant);
        var jonas = NewUser("Jonas Lund", "jonas@example.com", UserRole.Participant);

        Course NewCourse(string title, string description, User owner,
            DateTime start, DateTime end, string location, int capacity, CourseStatus status) => new()
        {
            Id = Guid.NewGuid(),
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

        var python = NewCourse("Grunnleggende Python", "En introduksjon til programmering med Python.",
            ida, Utc(2026, 12, 15, 9), Utc(2026, 12, 15, 14), "Sandefjord, rom A", 12, CourseStatus.Published);

        var sql = NewCourse("Introduksjon til SQL", "Lær å hente og endre data i relasjonsdatabaser.",
            omar, Utc(2026, 12, 16, 9), Utc(2026, 12, 16, 12), "Nettbasert", 2, CourseStatus.Published);

        var rest = NewCourse("REST API med ASP.NET Core", "Bygg et REST API med ASP.NET Core.",
            ida, Utc(2026, 12, 17, 9), Utc(2026, 12, 17, 15), "Sandefjord, rom B", 10, CourseStatus.Draft);

        var git = NewCourse("Git og GitHub", "Versjonskontroll og samarbeid med Git og GitHub.",
            omar, Utc(2026, 12, 18, 9), Utc(2026, 12, 18, 12), "Nettbasert", 8, CourseStatus.Cancelled);

        var docker = NewCourse("Docker for utviklere", "Containere og Docker i utviklingsarbeid.",
            omar, Utc(2026, 11, 10, 9), Utc(2026, 11, 10, 14), "Sandefjord, rom A", 10, CourseStatus.Published);

        var security = NewCourse("Sikkerhet i webapplikasjoner", "Vanlige trusler og hvordan du forsvarer deg mot dem.",
            ida, Utc(2026, 12, 21, 9), Utc(2026, 12, 21, 13), "Nettbasert", 6, CourseStatus.Published);

        Registration Active(Course course, User participant, DateTime at) => new()
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            ParticipantUserId = participant.Id,
            RegisteredAtUtc = at,
            ActiveParticipantUserId = participant.Id    // set only while the registration is active
        };

        Registration Cancelled(Course course, User participant, DateTime at, DateTime cancelledAt) => new()
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            ParticipantUserId = participant.Id,
            RegisteredAtUtc = at,
            CancelledAtUtc = cancelledAt,
            ActiveParticipantUserId = null              // cancelled: frees the spot
        };

        db.Users.AddRange(ida, omar, nora, emil, sara, jonas);
        db.Courses.AddRange(python, sql, rest, git, docker, security);
        db.Registrations.AddRange(
            Active(python, nora, Utc(2026, 11, 20, 10)),
            Active(python, emil, Utc(2026, 11, 20, 11)),
            Active(sql,    nora, Utc(2026, 11, 20, 12)),
            Active(sql,    sara, Utc(2026, 11, 20, 13)),
            Cancelled(security, jonas, Utc(2026, 11, 21, 9), Utc(2026, 11, 22, 9)));

        await db.SaveChangesAsync();
    }
}