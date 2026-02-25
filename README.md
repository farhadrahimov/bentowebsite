## Tortcu — Cake Website (ASP.NET Core MVC)

Minimalist, premium görünüşlü tort satış vebsaytı (PRD v1.0 əsasında).

### Tech

- **.NET 8** — ASP.NET Core MVC
- **EF Core** (model / query)
- **MSSQL**
- **DB schema**: versiyalı **SQL migration** faylları ilə (`db/migrations/sql/`)

### Local Setup

1. MSSQL-də yeni database yaradın (məs: `TortcuDb`)
2. İlkin schema-ni yaradın:
   - `db/migrations/sql/V1__InitialSchema.sql` faylını **SSMS** və ya `sqlcmd` ilə execute edin.
3. Connection string-i əlavə edin:
   - `appsettings.Development.json` (nümunə aşağıdadır)
4. Run:
   - `dotnet restore`
   - `dotnet run --project Tortcu.Web/Tortcu.Web.csproj`

### Solution

- `Tortcu.sln` daxilində 3 layihə var:
  - `Tortcu.Web` (MVC UI)
  - `Tortcu.Domain` (entities)
  - `Tortcu.Infrastructure` (DbContext + services)

### appsettings.Development.json (example)

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=TortcuDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Notes

- Logo üçün `wwwroot/images/logo.svg` placeholder var. İstəsəniz `wwwroot/images/logo.png` əlavə edib layout-da `img` path-ni dəyişə bilərsiniz.
- Online order aktiv deyil: `/order` səhifəsi **Coming Soon** göstərir (backend tərəfdə yalnız skeleton nəzərdə tutulub).

