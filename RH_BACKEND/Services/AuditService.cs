using System.Text.Json;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.Infrastructure;

namespace RH_BACKEND.Services;

public class AuditService(RhDbContext db, TenantContext tenant)
{
    public async Task WriteAsync(string action, string entityName, object entityId, object? oldValues = null, object? newValues = null)
    {
        db.AuditLogs.Add(new AuditLog
        {
            TenantId = tenant.CurrentTenantId,
            UserName = tenant.CurrentUserName,
            Action = action,
            EntityName = entityName,
            EntityId = entityId.ToString() ?? string.Empty,
            OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
            NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues)
        });

        await db.SaveChangesAsync();
    }
}
