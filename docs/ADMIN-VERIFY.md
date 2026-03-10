# Admin Panel — Yoxlama təlimatı

## Hazırlıq

### 1. Database (BENTOWEBSITE_DB)

- V1 və V2 skriptləri icra olunub.
- İlk admin yaradılıb:
  ```powershell
  cd Tools/SeedAdmin
  dotnet run -- "ÖzŞifrən"
  ```
  Çıxan `INSERT` əmrini SSMS-də işlədin.

### 2. Connection string

`appsettings.json`-da:
```json
"Default": "Server=DRACULA-FR;Database=BENTOWEBSITE_DB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. Run

```powershell
cd D:\PROJECTS\GitHub\BENTO\bentowebsite
dotnet run --project Tortcu.Web
```

Brauzerdə: `https://localhost:5xxx` (terminalda göstərilən porta baxın).

---

## Yoxlama addımları

### A. Login səhifəsi

| Addım | Əməliyyat | Gözlənilən |
|-------|-----------|------------|
| 1 | GET `/cp/auth` | Login formu (İstifadəçi adı, Şifrə, Daxil ol düyməsi) |
| 2 | Yanlış şifrə ilə POST | "Şifrə yanlışdır" mesajı |
| 3 | Mövcud olmayan istifadəçi | "İstifadəçi tapılmadı" mesajı |
| 4 | Düzgün admin / şifrə → Daxil ol | `/cp` və ya `/cp/dashboard`-a yönləndirmə |

### B. Dashboard (authorize)

| Addım | Əməliyyat | Gözlənilən |
|-------|-----------|------------|
| 5 | GET `/cp` (login olmadan) | `/cp/auth`-a yönləndirmə |
| 6 | Login olub GET `/cp` | "Salam, admin", Çıxış düyməsi, Phase placeholder-lar |
| 7 | Layout-da "Çıxış" | Form POST `/cp/auth/logout` → login səhifəsinə qayıt |

### C. Auth guard

| Addım | Əməliyyat | Gözlənilən |
|-------|-----------|------------|
| 8 | Cookie silinib GET `/cp` | Login səhifəsinə redirect |
| 9 | Yeni incognito pəncərədə `/cp` | Login səhifəsi |

### D. Rate limiting (login brute-force)

| Addım | Əməliyyat | Gözlənilən |
|-------|-----------|------------|
| 10 | `/cp/auth` üzərində 6+ dəfə sürətlə POST (yanlış şifrə) | 6-cı və sonrakı cavablarda **429 Too Many Requests** |
| 11 | 1 dəqiqə gözləyib yenidən cəhd | Login yenidən işləyir |

### E. Security headers

| Addım | Əməliyyat | Gözlənilən |
|-------|-----------|------------|
| 12 | Brauzer DevTools → Network → hər hansı səhifə → Response Headers | `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy` mövcuddur |

### F. robots.txt

| Addım | Əməliyyat | Gözlənilən |
|-------|-----------|------------|
| 13 | GET `/robots.txt` | `Disallow: /cp/` sətiri görünür |

---

## Tez checklist (DB ilə)

```
[ ] dotnet run
[ ] /cp/auth açılır
[ ] admin + şifrə ilə login
[ ] /cp dashboard göstərilir
[ ] Çıxış → login səhifəsi
[ ] Login olmadan /cp → redirect
[ ] 6 dəfə yanlış login → 429
[ ] Response headers-da X-Frame-Options
```

---

## Problem halında

| Problem | Həll |
|---------|------|
| "İstifadəçi tapılmadı" | AdminUser cədvəlində sətir varmı yoxla; `Tools/SeedAdmin` ilə INSERT edin |
| 404 /cp/auth | Route düzgündür; tətbiqi yenidən başladın |
| 500 runtime | Terminal stack trace; connection string və DB yoxlanması |
| 429 görmürəm | Rate limit 1 dəq sonra sıfırlanır; 6+ sürətli POST edin |
