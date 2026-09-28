# Torresoft People — RH_BACKEND

Backend de la versión 1 de Torresoft People, orientado a empresas privadas y entidades públicas.

## Stack

- .NET 8 Web API
- Entity Framework Core 8
- SQL Server
- JWT
- BCrypt
- Redis preparado para cache
- Swagger

## Módulos V1 incluidos

- Administración y multi-tenant
- Organización: unidades y cargos
- Catálogos
- Auditoría
- Personal
- Legajo digital
- Contratos
- Horarios y turnos
- Asistencia
- Vacaciones, permisos y licencias
- Workflow configurable
- Notificaciones
- Dashboard RR.HH.

## Ejecutar

1. Tener SQL Server LocalDB o cambiar `ConnectionStrings:DefaultConnection`.
2. Ejecutar:

```bash
dotnet restore
dotnet run --project RH_BACKEND/RH_BACKEND.csproj
```

3. Abrir Swagger en la URL que muestre la consola.

La configuración de desarrollo crea automáticamente la base y un tenant demo.

Usuario demo:

- Usuario: `admin`
- Contraseña: `Admin123!`
- Tenant: `11111111-1111-1111-1111-111111111111`

> Cambiar credenciales y clave JWT antes de producción.

## Multi-tenant

El API acepta opcionalmente:

```http
X-Tenant-Id: 11111111-1111-1111-1111-111111111111
```

Si no se envía, utiliza el tenant demo en desarrollo.

## Próximos pasos

- Migraciones EF Core
- almacenamiento S3/Blob real
- integración biométrica
- RabbitMQ para eventos y notificaciones
- firma digital
- separación progresiva por microservicios
