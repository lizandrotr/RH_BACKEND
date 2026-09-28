using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Infrastructure;

namespace RH_BACKEND.Services;

public class AttendanceService(RhDbContext db, TenantContext tenant)
{
    public async Task<object> GetDailySummaryAsync(DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var activeEmployees = await db.Employees.CountAsync(x =>
            x.TenantId == tenant.CurrentTenantId && x.Status == "ACTIVE");

        var employeeIds = await db.AttendanceMarks
            .Where(x => x.TenantId == tenant.CurrentTenantId && x.MarkedAt >= start && x.MarkedAt < end)
            .Select(x => x.EmployeeId)
            .Distinct()
            .ToListAsync();

        return new
        {
            date,
            activeEmployees,
            employeesWithMarks = employeeIds.Count,
            absent = Math.Max(0, activeEmployees - employeeIds.Count),
            totalMarks = await db.AttendanceMarks.CountAsync(x =>
                x.TenantId == tenant.CurrentTenantId && x.MarkedAt >= start && x.MarkedAt < end)
        };
    }
}
