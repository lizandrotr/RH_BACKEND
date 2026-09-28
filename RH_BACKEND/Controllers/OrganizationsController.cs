using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.dto;
using RH_BACKEND.Infrastructure;
using RH_BACKEND.Services;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/organization")]
public class OrganizationsController(RhDbContext db, TenantContext tenant, AuditService audit) : ControllerBase
{
    [HttpGet("units")]
    public Task<List<OrganizationUnit>> GetUnits() =>
        db.OrganizationUnits.Where(x => x.TenantId == tenant.CurrentTenantId)
            .OrderBy(x => x.Name).ToListAsync();

    [HttpPost("units")]
    public async Task<IActionResult> CreateUnit(CreateOrganizationUnitRequest request)
    {
        var entity = new OrganizationUnit
        {
            TenantId = tenant.CurrentTenantId,
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Type = request.Type.Trim().ToUpperInvariant(),
            ParentId = request.ParentId
        };

        db.OrganizationUnits.Add(entity);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(OrganizationUnit), entity.Id, newValues: entity);
        return CreatedAtAction(nameof(GetUnits), new { id = entity.Id }, entity);
    }

    [HttpGet("positions")]
    public Task<List<Position>> GetPositions() =>
        db.Positions.Where(x => x.TenantId == tenant.CurrentTenantId)
            .OrderBy(x => x.Name).ToListAsync();

    [HttpPost("positions")]
    public async Task<IActionResult> CreatePosition(CreatePositionRequest request)
    {
        var entity = new Position
        {
            TenantId = tenant.CurrentTenantId,
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            OrganizationUnitId = request.OrganizationUnitId
        };

        db.Positions.Add(entity);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(Position), entity.Id, newValues: entity);
        return Ok(entity);
    }
}
