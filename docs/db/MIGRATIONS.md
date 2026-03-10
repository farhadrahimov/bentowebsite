# Verilənlər Bazası Migrasiyaları

SQL migrasiya faylları: `db/migrations/sql/`

## İcra qaydası

| Versiya | Fayl | Məzmun |
|---------|------|--------|
| V1 | `V1__InitialSchema.sql` | Bütün əsas cədvəllər (Category, Product, ProductImage, Campaign, AboutContent, SeoMeta) |
| V2 | `V2__AdminUser.sql` | Admin autentifikasiya cədvəli |
| V3 | `V3__ProductImage_ShowInGallery.sql` | ProductImage.ShowInGallery sütunu |

Skriptləri ardıcıllıqla SSMS və ya `sqlcmd` ilə tətbiq edin:

```sql
-- SSMS-də: File → Open → hər V*.sql faylını açıb Execute
-- sqlcmd ilə:
sqlcmd -S .\SQLEXPRESS -d TortcuDb -i V1__InitialSchema.sql
sqlcmd -S .\SQLEXPRESS -d TortcuDb -i V2__AdminUser.sql
sqlcmd -S .\SQLEXPRESS -d TortcuDb -i V3__ProductImage_ShowInGallery.sql
```

## Qeydlər

- Skriptlər **idempotentdir** — `IF NOT EXISTS` yoxlamaları ilə təkrar icra təhlükəsizdir.
- `NVARCHAR(MAX)` uzun mətnlər üçün (məsələn `AboutContent.Content`).
- Yeni migrasiya əlavə edərkən versiya nömrəsini ardıcıl artırın: `V4__...`

## Flyway (avtomatik)

Flyway və ya oxşar alət işlədirsinizsə, faylları `db/migrations/sql/` qovluğunda saxlamaq kifayətdir — adlandırma qaydası (`V{n}__{açıqlama}.sql`) avtomatik aşkarlamağa imkan verir.
