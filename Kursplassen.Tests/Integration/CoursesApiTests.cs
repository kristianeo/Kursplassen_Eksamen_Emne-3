using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using Kursplassen_Eksamen_Emne_3;
using Kursplassen_Eksamen_Emne_3.Dtos;
using Kursplassen_Eksamen_Emne_3.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit.Abstractions;

namespace Kursplassen.Tests.Integration;

public class CoursesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    public CoursesApiTests(WebApplicationFactory<Program> factory,
        ITestOutputHelper output)
    {
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<KursplassenDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<KursplassenDbContext>>();
                services.AddDbContext<KursplassenDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        });
        _client = customFactory.CreateClient();
        _output = output;
    }

    private CreateCourseDto CreateCourse()
    {
        return new CreateCourseDto
        {
            Title = "Tester",
            Description = "Test",
            StartsAtUtc = DateTime.UtcNow.AddDays(1),
            EndsAtUtc = DateTime.UtcNow.AddDays(2),
            Location = "Test",
            Capacity = 10,
            Status = CourseStatus.Published
        };
    }
    
    private async Task<string> GetTokenAsync(string email)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto { Email = email, Password = "Test1234!" });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return body!.Token;
    }
    
    [Fact]
    public async Task GetAll_ReturnsSuccessAndCourses()
    {
        var response = await _client.GetAsync("/api/courses");
        response.EnsureSuccessStatusCode();
        var courses = await response.Content.ReadFromJsonAsync<CourseSummaryDto[]>();
        
        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine(body);
        
        Assert.NotNull(courses);
    }
    [Fact]
    public async Task Create_InvalidDate_ReturnsBadRequest()
    {
        var token = await GetTokenAsync("ida@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var course = CreateCourse();
        course.StartsAtUtc = DateTime.UtcNow.AddDays(-1);
        
        var response = await _client.PostAsJsonAsync("/api/courses", course);
        
        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine(body);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutAuthorization_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/courses",
            CreateCourse());
        
        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine(body);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_AsParticipant_ReturnsForbidden()
    {
        // Nora is participant
        var token = await GetTokenAsync("nora@example.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await _client.PostAsJsonAsync("/api/courses",
            CreateCourse());
        
        var body = await response.Content.ReadAsStringAsync();
        _output.WriteLine(body);
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}