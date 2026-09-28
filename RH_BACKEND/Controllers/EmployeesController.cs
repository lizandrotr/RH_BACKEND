using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.dto;
using RH_BACKEND.Infrastructure;
using RH_BACKEND.Services;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController(RhDbContext db, TenantContext tenant, AuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var query = db.Employees.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x =>
                x.DocumentNumber.Contains(value) ||
                x.FirstName.Contains(value) ||
                x.LastName.Contains(value) ||
                x.EmployeeCode.Contains(value));
        }

        return Ok(await query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var employee = await db.Employees.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenant.CurrentTenantId);

        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeRequest request)
    {
        var entity = new Employee
        {
            TenantId = tenant.CurrentTenantId,
            EmployeeCode = request.EmployeeCode.Trim(),
            DocumentNumber = request.DocumentNumber.Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone?.Trim(),
            HireDate = request.HireDate,
            OrganizationUnitId = request.OrganizationUnitId,
            PositionId = request.PositionId,
            ManagerEmployeeId = request.ManagerEmployeeId
        };

        db.Employees.Add(entity);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(Employee), entity.Id, newValues: entity);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateEmployeeRequest request)
    {
        var entity = await db.Employees.FirstOrDefaultAsync(x =>
            x.Id == id && x.TenantId == tenant.CurrentTenantId);

        if (entity is null) return NotFound();

        var before = new
        {
            entity.FirstName, entity.LastName, entity.Email, entity.Phone, entity.Status,
            entity.OrganizationUnitId, entity.PositionId, entity.ManagerEmployeeId
        };

        entity.FirstName = request.FirstName.Trim();
        entity.LastName = request.LastName.Trim();
        entity.Email = request.Email.Trim();
        entity.Phone = request.Phone?.Trim();
        entity.Status = request.Status.Trim().ToUpperInvariant();
        entity.OrganizationUnitId = request.OrganizationUnitId;
        entity.PositionId = request.PositionId;
        entity.ManagerEmployeeId = request.ManagerEmployeeId;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        await audit.WriteAsync("UPDATE", nameof(Employee), entity.Id, before, entity);
        return Ok(entity);
    }

    [HttpGet("{employeeId:guid}/documents")]
    public Task<List<EmployeeDocument>> GetDocuments(Guid employeeId) =>
        db.EmployeeDocuments.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId && x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAt).ToListAsync();

    [HttpPost("{employeeId:guid}/documents")]
    public async Task<IActionResult> AddDocument(Guid employeeId, CreateDocumentRequest request)
    {
        var entity = new EmployeeDocument
        {
            TenantId = tenant.CurrentTenantId,
            EmployeeId = employeeId,
            DocumentType = request.DocumentType,
            FileName = request.FileName,
            StorageUrl = request.StorageUrl,
            IssueDate = request.IssueDate,
            ExpirationDate = request.ExpirationDate,
            Notes = request.Notes
        };

        db.EmployeeDocuments.Add(entity);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(EmployeeDocument), entity.Id, newValues: entity);
        return Ok(entity);
    }

    [HttpGet("{employeeId:guid}/contracts")]
    public Task<List<EmploymentContract>> GetContracts(Guid employeeId) =>
        db.EmploymentContracts.AsNoTracking()
            .Where(x => x.TenantId == tenant.CurrentTenantId && x.EmployeeId == employeeId)
            .OrderByDescending(x => x.StartDate).ToListAsync();

    [HttpPost("{employeeId:guid}/contracts")]
    public async Task<IActionResult> AddContract(Guid employeeId, CreateContractRequest request)
    {
        var entity = new EmploymentContract
        {
            TenantId = tenant.CurrentTenantId,
            EmployeeId = employeeId,
            ContractType = request.ContractType,
            Number = request.Number,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MonthlyAmount = request.MonthlyAmount,
            DocumentUrl = request.DocumentUrl
        };

        db.EmploymentContracts.Add(entity);
        await db.SaveChangesAsync();
        await audit.WriteAsync("CREATE", nameof(EmploymentContract), entity.Id, newValues: entity);
        return Ok(entity);
    }
}
