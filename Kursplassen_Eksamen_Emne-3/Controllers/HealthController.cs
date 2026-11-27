using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kursplassen_Eksamen_Emne_3.Controllers;

[ApiController]
[Route("health")]
public class HealthController(HealthCheckService healthChecks) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        var report = await healthChecks.CheckHealthAsync();
        var body = new { status = report.Status.ToString() };

        return report.Status == HealthStatus.Healthy
            ? Ok(body)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, body);
    }
}