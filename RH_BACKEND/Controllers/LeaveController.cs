using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.dto;
using RH_BACKEND.Infrastructure;
using RH_BACKEND.Services;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/leave")]
public class LeaveController(RhDbContext db, TenantContext tenant, AuditService audit) : ControllerBase
{
    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests([FromQuery] string? status = null)
    {
        var query = db.LeaveRequests.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status.ToUpperInvariant());

        return Ok(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    [HttpPost("requests")]
    public async Task<IActionResult> CreateRequest(CreateLeaveRequest request)
    {
        if (request.EndDate < request.StartDate)
            return BadRequest(new { message = "La fecha fin no puede ser menor a la fecha inicio." });

        var entity = new LeaveRequest
        {
            TenantId = tenant.CurrentTenantId,
            EmployeeId = request.EmployeeId,
            Type = request.Type.ToUpperInvariant(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RequestedDays = request.RequestedDays,
            Reason = request.Reason,
            ApproverEmployeeId = request.ApproverEmployeeId
        };

        db.LeaveRequests.Add(entity);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(LeaveRequest), entity.Id, newValues: entity);
        return Ok(entity);
    }

    [HttpPut("requests/{id:guid}/review")]
    public async Task<IActionResult> Review(Guid id, ReviewLeaveRequest request)
    {
        var entity = await db.LeaveRequests.FirstOrDefaultAsync(x =>
            x.Id == id && x.TenantId == tenant.CurrentTenantId);

        if (entity is null) return NotFound();

        var normalized = request.Status.ToUpperInvariant();
        if (normalized is not ("APPROVED" or "REJECTED" or "OBSERVED"))
            return BadRequest(new { message = "Estado de revisión inválido." });

        var before = new { entity.Status, entity.ReviewComment };
        entity.Status = normalized;
        entity.ReviewComment = request.Comment;
        entity.ReviewedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        db.Notifications.Add(new Notification
        {
            TenantId = tenant.CurrentTenantId,
            EmployeeId = entity.EmployeeId,
            Title = "Solicitud actualizada",
            Message = $"Tu solicitud {entity.Type} fue {normalized.ToLowerInvariant()}."
        });

        await db.SaveChangesAsync();
        await audit.WriteAsync("REVIEW", nameof(LeaveRequest), entity.Id, before, entity);
        return Ok(entity);
    }
}
