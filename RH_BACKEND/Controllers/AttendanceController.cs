using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.dto;
using RH_BACKEND.Infrastructure;
using RH_BACKEND.Services;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/attendance")]
public class AttendanceController(
    RhDbContext db,
    TenantContext tenant,
    AuditService audit,
    AttendanceService attendanceService) : ControllerBase
{
    [HttpGet("schedules")]
    public Task<List<WorkSchedule>> GetSchedules() =>
        db.WorkSchedules.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId && x.IsActive)
            .OrderBy(x => x.Name).ToListAsync();

    [HttpPost("schedules")]
    public async Task<IActionResult> CreateSchedule(CreateScheduleRequest request)
    {
        var schedule = new WorkSchedule
        {
            TenantId = tenant.CurrentTenantId,
            Name = request.Name,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            ToleranceMinutes = request.ToleranceMinutes,
            BreakMinutes = request.BreakMinutes
        };

        db.WorkSchedules.Add(schedule);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(WorkSchedule), schedule.Id, newValues: schedule);
        return Ok(schedule);
    }

    [HttpPost("schedule-assignments")]
    public async Task<IActionResult> AssignSchedule(AssignScheduleRequest request)
    {
        var assignment = new EmployeeSchedule
        {
            TenantId = tenant.CurrentTenantId,
            EmployeeId = request.EmployeeId,
            WorkScheduleId = request.WorkScheduleId,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        db.EmployeeSchedules.Add(assignment);
        await db.SaveChangesAsync();
        return Ok(assignment);
    }

    [HttpPost("marks")]
    public async Task<IActionResult> RegisterMark(CreateAttendanceMarkRequest request)
    {
        var mark = new AttendanceMark
        {
            TenantId = tenant.CurrentTenantId,
            EmployeeId = request.EmployeeId,
            MarkedAt = request.MarkedAt,
            Type = request.Type.ToUpperInvariant(),
            Origin = request.Origin.ToUpperInvariant(),
            DeviceId = request.DeviceId,
            Notes = request.Notes
        };

        db.AttendanceMarks.Add(mark);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(AttendanceMark), mark.Id, newValues: mark);
        return Ok(mark);
    }

    [HttpGet("marks")]
    public async Task<IActionResult> GetMarks([FromQuery] DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var marks = await db.AttendanceMarks.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId && x.MarkedAt >= start && x.MarkedAt < end)
            .OrderBy(x => x.MarkedAt)
            .ToListAsync();

        return Ok(marks);
    }

    [HttpGet("summary")]
    public Task<object> GetSummary([FromQuery] DateOnly date) =>
        attendanceService.GetDailySummaryAsync(date);
}
