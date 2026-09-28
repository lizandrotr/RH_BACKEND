using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.dto;
using RH_BACKEND.Infrastructure;
using RH_BACKEND.Services;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/configuration")]
public class ConfigurationController(RhDbContext db, TenantContext tenant, AuditService audit) : ControllerBase
{
    [HttpGet("catalogs")]
    public async Task<IActionResult> GetCatalogs([FromQuery] string? category = null)
    {
        var query = db.CatalogItems.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId && x.IsActive);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(x => x.Category == category.ToUpperInvariant());

        return Ok(await query.OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync());
    }

    [HttpPost("catalogs")]
    public async Task<IActionResult> CreateCatalog(CreateCatalogItemRequest request)
    {
        var item = new CatalogItem
        {
            TenantId = tenant.CurrentTenantId,
            Category = request.Category.Trim().ToUpperInvariant(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim()
        };

        db.CatalogItems.Add(item);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(CatalogItem), item.Id, newValues: item);
        return Ok(item);
    }

    [HttpGet("audit")]
    public async Task<IActionResult> GetAudit([FromQuery] int take = 100) =>
        Ok(await db.AuditLogs.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId)
            .OrderByDescending(x => x.OccurredAt)
            .Take(Math.Clamp(take, 1, 500))
            .ToListAsync());
}
