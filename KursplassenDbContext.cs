using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Kursplassen_Eksamen_Emne_3;

public class KursplassenDbContext(DbContextOptions<KursplassenDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Registration> Registrations => Set<Registration>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>()
            .Property(p => p.Id)
            .IsRequired()
            .HasMaxLength(200);
    }

}