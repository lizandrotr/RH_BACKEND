using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.dto;
using RH_BACKEND.Infrastructure;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/workflows")]
public class WorkflowController(RhDbContext db, TenantContext tenant) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var definitions = await db.WorkflowDefinitions.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(definitions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkflowRequest request)
    {
        var definition = new WorkflowDefinition
        {
            TenantId = tenant.CurrentTenantId,
            Name = request.Name,
            EntityType = request.EntityType.ToUpperInvariant()
        };

        db.WorkflowDefinitions.Add(definition);
        await db.SaveChangesAsync();

        var steps = request.Steps.Select(x => new WorkflowStep
        {
            WorkflowDefinitionId = definition.Id,
            StepOrder = x.StepOrder,
            Name = x.Name,
            ApproverRole = x.ApproverRole
        });

        db.WorkflowSteps.AddRange(steps);
        await db.SaveChangesAsync();

        return Ok(new { definition, steps = await db.WorkflowSteps.Where(x => x.WorkflowDefinitionId == definition.Id).ToListAsync() });
    }
}
