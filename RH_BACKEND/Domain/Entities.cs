namespace RH_BACKEND.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class UserAccount : TenantEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "HR_ADMIN";
    public bool IsActive { get; set; } = true;
}

public class CatalogItem : TenantEntity
{
    public string Category { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class OrganizationUnit : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "AREA";
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Position : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? OrganizationUnitId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Employee : TenantEntity
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public Guid? OrganizationUnitId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? ManagerEmployeeId { get; set; }
}

public class EmployeeDocument : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public string? Notes { get; set; }
}

public class EmploymentContract : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? MonthlyAmount { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public string? DocumentUrl { get; set; }
}

public class WorkSchedule : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int ToleranceMinutes { get; set; }
    public int BreakMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;
}

public class EmployeeSchedule : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public Guid WorkScheduleId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class AttendanceMark : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public DateTime MarkedAt { get; set; }
    public string Type { get; set; } = "CHECK";
    public string Origin { get; set; } = "WEB";
    public string? DeviceId { get; set; }
    public string? Notes { get; set; }
}

public class LeaveRequest : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public string Type { get; set; } = "VACATION";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal RequestedDays { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "PENDING";
    public Guid? ApproverEmployeeId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }
}

public class WorkflowDefinition : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class WorkflowStep : BaseEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public int StepOrder { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApproverRole { get; set; } = string.Empty;
}

public class WorkflowInstance : TenantEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public int CurrentStep { get; set; } = 1;
    public string Status { get; set; } = "PENDING";
}

public class Notification : TenantEntity
{
    public Guid? EmployeeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime? ReadAt { get; set; }
}

public class AuditLog : TenantEntity
{
    public string UserName { get; set; } = "system";
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
}
