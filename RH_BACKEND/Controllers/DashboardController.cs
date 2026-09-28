using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Infrastructure;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(RhDbContext db, TenantContext tenant) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var start = today.ToDateTime(TimeOnly.MinValue);
        var end = today.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var next30 = today.AddDays(30);

        var activeEmployees = await db.Employees.CountAsync(x =>
            x.TenantId == tenant.CurrentTenantId && x.Status == "ACTIVE");

        var presentToday = await db.AttendanceMarks
            .Where(x => x.TenantId == tenant.CurrentTenantId && x.MarkedAt >= start && x.MarkedAt < end)
            .Select(x => x.EmployeeId)
            .Distinct()
            .CountAsync();

        var pendingRequests = await db.LeaveRequests.CountAsync(x =>
            x.TenantId == tenant.CurrentTenantId && x.Status == "PENDING");

        var employeesOnLeave = await db.LeaveRequests.CountAsync(x =>
            x.TenantId == tenant.CurrentTenantId &&
            x.Status == "APPROVED" &&
            x.StartDate <= today &&
            x.EndDate >= today);

        var expiringContracts = await db.EmploymentContracts.CountAsync(x =>
            x.TenantId == tenant.CurrentTenantId &&
            x.Status == "ACTIVE" &&
            x.EndDate != null &&
            x.EndDate >= today &&
            x.EndDate <= next30);

        return Ok(new
        {
            activeEmployees,
            presentToday,
            absentToday = Math.Max(0, activeEmployees - presentToday),
            employeesOnLeave,
            pendingRequests,
            expiringContracts
        });
    }
}
