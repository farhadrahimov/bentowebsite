# Lokal yoxlama (database olmadan)

## Hazırlıq

1. **İşləyən Tortcu.Web prosesi varsa** — dayandır (Ctrl+C və ya taskkill).
2. `appsettings.json`-da `ConnectionStrings.Default` **boş** qalsın — tətbiq **InMemory** istifadə edəcək.

---

## Run

```powershell
cd D:\PROJECTS\GitHub\BENTO\bentowebsite
dotnet run --project Tortcu.Web
```

Brauzerdə:
- `https://localhost:5xxx` və ya `http://localhost:5xxx` (terminalda göstərilən porta bax).

---

## Yoxlama checklist

| # | URL | Gözlənilən nəticə |
|---|-----|-------------------|
| 1 | `/` | Ana səhifə açılır |
| 2 | `/products` | Məhsullar səhifəsi |
| 3 | `/about` | Haqqımızda |
| 4 | `/contact` | Əlaqə |
| 5 | `/cp/auth` | Login formu (İstifadəçi adı, Şifrə) |
| 6 | `/cp/auth` → admin / Test123 → Daxil ol | Dashboard-a yönləndirmə |
| 7 | `/cp` | Panel (salam, admin) |
| 8 | `/cp/auth` → Çıxış | Login səhifəsinə qayıt |

---

## Test admin (yalnız InMemory + Development)

- **İstifadəçi:** `admin`
- **Şifrə:** `Test123`

Bu seed **yalnız** Connection string boş və Development mühitində işləyəndə əlavə olunur. Production və ya real DB ilə **istifadə olunmur**.

---

## Problem halında

- **Login "İstifadəçi tapılmadı"** — InMemory işlədiyindən əmin ol; connection string boş olmalıdır.
- **500 / runtime error** — terminaldakı stack trace-ə bax.
- **Səhifə tapılmadı (404)** — route `/cp/auth` və `/cp` düzgün yazılıb, `dotnet run` yenidən işə sal.

---

## Ətraflı admin yoxlama

Real database ilə admin paneli yoxlamaq üçün: **docs/ADMIN-VERIFY.md**
