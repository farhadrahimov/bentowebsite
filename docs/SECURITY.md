# Security

## Rate limiting

| Policy | Limit | Scope |
|--------|-------|-------|
| **login** | 5 req/min per IP | POST /cp/auth |
| **global** | 120 req/min per IP | Bütün request-lər |

Limit aşılanda: **429 Too Many Requests**.

## Security headers

Bütün cavablarda tətbiq olunur:

- **X-Content-Type-Options:** nosniff
- **X-Frame-Options:** DENY
- **Referrer-Policy:** strict-origin-when-cross-origin

## Digər

- CSRF: Login/logout-da AntiForgeryToken
- Cookie: HttpOnly, admin auth
- Şifrə: BCrypt (work factor 12)
- robots.txt: /cp/ bloklanıb
