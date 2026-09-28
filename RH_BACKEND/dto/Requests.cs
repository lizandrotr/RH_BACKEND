namespace RH_BACKEND.dto;

public record LoginRequest(string Username, string Password);

public record CreateOrganizationUnitRequest(string Code, string Name, string Type, Guid? ParentId);
public record CreatePositionRequest(string Code, string Name, Guid? OrganizationUnitId);

public record CreateEmployeeRequest(
    string EmployeeCode,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    DateOnly HireDate,
    Guid? OrganizationUnitId,
    Guid? PositionId,
    Guid? ManagerEmployeeId);

public record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Status,
    Guid? OrganizationUnitId,
    Guid? PositionId,
    Guid? ManagerEmployeeId);

public record CreateDocumentRequest(
    string DocumentType,
    string FileName,
    string StorageUrl,
    DateOnly? IssueDate,
    DateOnly? ExpirationDate,
    string? Notes);

public record CreateContractRequest(
    string ContractType,
    string Number,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal? MonthlyAmount,
    string? DocumentUrl);

public record CreateScheduleRequest(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int ToleranceMinutes,
    int BreakMinutes);

public record AssignScheduleRequest(Guid EmployeeId, Guid WorkScheduleId, DateOnly StartDate, DateOnly? EndDate);

public record CreateAttendanceMarkRequest(
    Guid EmployeeId,
    DateTime MarkedAt,
    string Type,
    string Origin,
    string? DeviceId,
    string? Notes);

public record CreateLeaveRequest(
    Guid EmployeeId,
    string Type,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal RequestedDays,
    string? Reason,
    Guid? ApproverEmployeeId);

public record ReviewLeaveRequest(string Status, string? Comment);

public record CreateCatalogItemRequest(string Category, string Code, string Name);

public record CreateWorkflowRequest(string Name, string EntityType, IReadOnlyList<WorkflowStepRequest> Steps);
public record WorkflowStepRequest(int StepOrder, string Name, string ApproverRole);
