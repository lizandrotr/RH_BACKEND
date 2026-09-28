using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Domain;

namespace RH_BACKEND.Data;

public class RhDbContext(DbContextOptions<RhDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
    public DbSet<EmploymentContract> EmploymentContracts => Set<EmploymentContract>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();
    public DbSet<EmployeeSchedule> EmployeeSchedules => Set<EmployeeSchedule>();
    public DbSet<AttendanceMark> AttendanceMarks => Set<AttendanceMark>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<UserAccount>().HasIndex(x => new { x.TenantId, x.Username }).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => new { x.TenantId, x.DocumentNumber }).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => new { x.TenantId, x.EmployeeCode }).IsUnique();
        modelBuilder.Entity<CatalogItem>().HasIndex(x => new { x.TenantId, x.Category, x.Code }).IsUnique();
        modelBuilder.Entity<OrganizationUnit>().HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        modelBuilder.Entity<Position>().HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        modelBuilder.Entity<WorkSchedule>().HasIndex(x => new { x.TenantId, x.Name });
        modelBuilder.Entity<AttendanceMark>().HasIndex(x => new { x.TenantId, x.EmployeeId, x.MarkedAt });
        modelBuilder.Entity<LeaveRequest>().HasIndex(x => new { x.TenantId, x.EmployeeId, x.StartDate });

        base.OnModelCreating(modelBuilder);
    }
}
