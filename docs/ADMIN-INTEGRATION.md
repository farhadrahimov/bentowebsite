# Admin Panel Integration Plan

**Status:** Planning & Analysis  
**Last updated:** 2025-02-25  
**Approach:** Phased, professional, security-conscious

---

## Step 0: Schema & Codebase Analysis (Completed 2025-02-25)

### 1. Current Database Schema (`db/migrations/sql/V1__InitialSchema.sql`)

| Table | Purpose | Admin-relevant fields |
|-------|---------|------------------------|
| **Category** | Məhsul kateqoriyaları | Name, Slug, IsActive, DisplayOrder |
| **Product** | Məhsullar | Name, Slug, Description, Price, IsActive, IsPopular, CategoryId |
| **ProductImage** | Məhsul şəkilləri | ProductId, ImageUrl, IsPrimary, DisplayOrder |
| **Campaign** | Ana səhifə kampaniya banneri | Title, SubTitle, ImageUrl, StartDateUtc, EndDateUtc, IsActive |
| **AboutContent** | Haqqımızda səhifəsi kontenti | Content, MainImageUrl |
| **SeoMeta** | SEO meta (title, description, OG) | PageType, EntityId, MetaTitle, MetaDescription, MetaKeywords, OgTitle, OgDescription, OgImageUrl |

**Not:** Hazırda **authentication/authorization** üçün heç bir cədvəl yoxdur. Admin panel üçün gələcəkdə `AdminUser` və ya `AspNetUsers` (Identity) əlavə olunmalıdır.

---

### 2. Admin Modules (PRD əsasən)

| Module | Entity(-lər) | CRUD | Notes |
|--------|--------------|------|-------|
| **Products & Categories** | Category, Product, ProductImage | Full | Slug avtomatik/manuel, IsPopular flag |
| **Campaign Banner** | Campaign | Full | Ana səhifədə yalnız 1 aktiv kampaniya |
| **About Page** | AboutContent | Update | Tek sətir kontent (Id=1 və ya son) |
| **SEO Meta** | SeoMeta | Update per page | PageType + EntityId birləşməsi unique |

---

### 3. Route & Security Strategy

| Requirement | Decision |
|-------------|----------|
| **Admin base path** | `/cp` (Control Panel) — `/admin` istifadə edilməsin, axtarışda tapılmasın |
| **Login path** | `/cp/auth` — sadə, qısa, obyektiv |
| **Discovery** | robots.txt-də `/cp` bloklanacaq; sitemap-da olmayacaq |
| **Auth** | ASP.NET Core Identity və ya custom cookie-based (senior yanaşma) |

---

### 4. Planned Phases

| Phase | Scope | DB changes | Deliverable |
|-------|-------|------------|-------------|
| **Phase 1** | Auth foundation | V2: AdminUser cədvəli | Login/logout, cookie auth, `/cp` guard |
| **Phase 2** | Products & Categories | — | CRUD UI, slug, images |
| **Phase 3** | Campaign Banner | — | CRUD, aktiv/deaktiv, tarix |
| **Phase 4** | About + SEO Meta | — | Form-lar, PageType mapping |
| **Phase 5** | Polish | — | Audit, robots, documentation |

---

### 5. Current Codebase Touchpoints

- **Controllers:** `HomeController`, `ProductsController`, `AboutController` — data `AppDbContext` vasitəsilə oxunur.
- **Services:** `SeoMetaService`, `SitemapService` — public site üçün.
- **Layout:** Heç bir admin link yoxdur (yaxşı).

---

## Step Log

| Step | Date | Action | Notes |
|------|------|--------|-------|
| 0 | 2025-02-25 | Schema analysis, plan doc | Bu sənəd yaradıldı |
| 1 | 2025-02-25 | Phase 1: Auth foundation | AdminUser, cookie auth, /cp area, robots |
| — | 2025-02-25 | SQL fix | AboutContent.Content: NVARCHAR(12000) → NVARCHAR(MAX) (MSSQL limit 4000) |
| 2 | 2025-02-25 | Phase 2: Products & Categories CRUD | Kateqoriya və məhsul siyahı/create/edit/delete |
| 3 | 2025-02-25 | Phase 3: Campaign CRUD | Limitlər: 1 aktiv, Title≤200, SubTitle≤400, 1 şəkil |
| 4 | 2025-02-25 | Phase 4: About edit | Content≤15k simvol, 1 MainImage |

---

## Phase 1 Summary (Step 1)

**Əlavə olunan fayllar:**
- `db/migrations/sql/V2__AdminUser.sql` — AdminUser cədvəli
- `db/migrations/sql/README-SEED-ADMIN.md` — ilk admin seed təlimatı
- `Tools/SeedAdmin/` — BCrypt hash generator (dotnet run)
- `Tortcu.Domain/AdminUser.cs` — entity
- `Tortcu.Infrastructure/Services/IAdminAuthService.cs`, `AdminAuthService.cs` — validasiya
- `Tortcu.Web/Areas/Cp/` — area, AuthController, DashboardController, Login/Dashboard views

**Route-lar:**
- `GET /cp` — dashboard (authorize)
- `GET /cp/auth` — login form
- `POST /cp/auth` — login
- `POST /cp/auth/logout` — çıxış

**Təhlükəsizlik:**
- Cookie auth (`.Tortcu.Cp`), 8 saat sliding expiration
- robots.txt: `Disallow: /cp/`
- View meta: `noindex, nofollow`

---

## Phase 2 Summary (2025-02-25)

**Əlavə olunan fayllar:**
- `Tortcu.Web/Areas/Cp/Controllers/CategoriesController.cs` — CRUD
- `Tortcu.Web/Areas/Cp/Controllers/ProductsController.cs` — CRUD
- `Tortcu.Web/Areas/Cp/Models/CategoryListModel.cs`
- `Tortcu.Web/Areas/Cp/Views/Categories/` — Index, Create, Edit
- `Tortcu.Web/Areas/Cp/Views/Products/` — Index, Create, Edit

**Route-lar:**
- `GET/POST /cp/categories` — list, create
- `GET/POST /cp/categories/edit/{id}` — edit
- `POST /cp/categories/delete/{id}` — delete
- `GET/POST /cp/products` — list (filter by category)
- `GET/POST /cp/products/create` — create
- `GET/POST /cp/products/edit/{id}` — edit
- `POST /cp/products/delete/{id}` — delete

**Funksionallıq:**
- Slug avtomatik (ad əsasında) və ya əl ilə, unikallıq yoxlanır
- Məhsul üçün 1 əsas şəkil URL
- Kateqoriyada məhsul varsa silinmir

---

## Phase 3 & 4 Summary (2025-02-25)

**Kampaniya (Phase 3):**
- Controller: `CampaignsController` — list, create, edit, delete
- Limitlər: Title max 200, SubTitle max 400, 1 şəkil (file/URL), yalnız 1 aktiv kampaniya
- Fayllar: `uploads/campaigns/`
- Ana səhifədə aktiv kampaniya şəkillə göstərilir

**Haqqımızda (Phase 4):**
- Controller: `AboutController` — edit (tək sətir)
- Limitlər: Content max 15 000 simvol, 1 MainImage
- Fayllar: `uploads/about/`

---

## Next Step

**Phase 5:** Polish, SEO Meta form-ları, audit.
