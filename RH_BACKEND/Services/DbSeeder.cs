using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.Domain;
using RH_BACKEND.Infrastructure;

namespace RH_BACKEND.Services;

public static class DbSeeder
{
    public static async Task SeedAsync(RhDbContext db)
    {
        var tenantId = TenantContext.DemoTenantId;

        if (!await db.Tenants.AnyAsync(x => x.Id == tenantId))
        {
            db.Tenants.Add(new Tenant
            {
                Id = tenantId,
                Code = "DEMO",
                Name = "Torresoft Demo"
            });
        }

        if (!await db.Users.AnyAsync(x => x.TenantId == tenantId))
        {
            db.Users.Add(new UserAccount
            {
                TenantId = tenantId,
                Username = "admin",
                FullName = "Administrador RR.HH.",
                Email = "admin@torresoft.local",
                Role = "HR_ADMIN",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            });
        }

        if (!await db.CatalogItems.AnyAsync(x => x.TenantId == tenantId))
        {
            db.CatalogItems.AddRange(
                new CatalogItem { TenantId = tenantId, Category = "CONTRACT_TYPE", Code = "INDEFINITE", Name = "Indefinido" },
                new CatalogItem { TenantId = tenantId, Category = "CONTRACT_TYPE", Code = "FIXED", Name = "Plazo fijo" },
                new CatalogItem { TenantId = tenantId, Category = "CONTRACT_TYPE", Code = "CAS", Name = "CAS" },
                new CatalogItem { TenantId = tenantId, Category = "LEAVE_TYPE", Code = "VACATION", Name = "Vacaciones" },
                new CatalogItem { TenantId = tenantId, Category = "LEAVE_TYPE", Code = "PERMISSION", Name = "Permiso" },
                new CatalogItem { TenantId = tenantId, Category = "LEAVE_TYPE", Code = "MEDICAL", Name = "Descanso médico" },
                new CatalogItem { TenantId = tenantId, Category = "DOCUMENT_TYPE", Code = "DNI", Name = "DNI" },
                new CatalogItem { TenantId = tenantId, Category = "DOCUMENT_TYPE", Code = "CV", Name = "Currículum Vitae" }
            );
        }

        await db.SaveChangesAsync();
    }
}
