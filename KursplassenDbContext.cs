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
        modelBuilder.Entity<Course>(course =>
        {
            course.Property(c => c.Title)
                .HasMaxLength(180);
            course.Property(c => c.Description)
                .HasColumnType("text");
            course.Property(c => c.Location)
                .HasMaxLength(180);
            course.Property(c => c.Capacity)
                .HasColumnType("int unsigned");
            course.Property(c => c.Status)
                .HasConversion<string>()
                .HasColumnType("enum('Draft','Published','Cancelled')");
            
            course.HasOne(c => c.Owner)
                .WithMany(u => u.OwnedCourses)
                .HasForeignKey(c => c.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.Email)
                .HasMaxLength(254);
            user.Property(u => u.PasswordHash)
                .HasMaxLength(255);
            user.Property(u => u.DisplayName)
                .HasMaxLength(120);
            user.Property(u => u.Role)
                .HasConversion<string>()
                .HasColumnType("enum('Participant','Instructor')");
        });

        modelBuilder.Entity<Registration>(registration =>
        {
            registration.HasOne(r => r.Course)
                .WithMany(c => c.Registrations)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            registration.HasIndex(r => new { r.CourseId, r.ActiveParticipantUserId })
                .IsUnique();
        });
    }

}