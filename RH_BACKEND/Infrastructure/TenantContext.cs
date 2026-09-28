namespace RH_BACKEND.Infrastructure;

public class TenantContext(IHttpContextAccessor httpContextAccessor)
{
    public static readonly Guid DemoTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid CurrentTenantId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            return Guid.TryParse(value, out var tenantId) ? tenantId : DemoTenantId;
        }
    }

    public string CurrentUserName =>
        httpContextAccessor.HttpContext?.User?.Identity?.Name
        ?? httpContextAccessor.HttpContext?.Request.Headers["X-User"].FirstOrDefault()
        ?? "system";
}
