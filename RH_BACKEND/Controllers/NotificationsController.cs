using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Infrastructure;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController(RhDbContext db, TenantContext tenant) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? employeeId = null)
    {
        var query = db.Notifications.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId);

        if (employeeId.HasValue)
            query = query.Where(x => x.EmployeeId == employeeId);

        return Ok(await query.OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync());
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(x =>
            x.Id == id && x.TenantId == tenant.CurrentTenantId);

        if (notification is null) return NotFound();

        notification.ReadAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
