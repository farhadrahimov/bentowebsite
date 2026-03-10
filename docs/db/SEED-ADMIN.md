# İlk Admin İstifadəçisinin Yaradılması

`V2__AdminUser.sql` tətbiq edildikdən sonra admin hesabı əlavə etmək üçün.

## BCrypt hash yaratmaq

Layihənin root qovluğundan:

```powershell
cd Tools/SeedAdmin
dotnet run -- "SizinŞifrə123!"
```

Bu əmr SQL `INSERT` ifadəsini çap edəcək. Nəticəni aşağıdakı addımda istifadə edin.

Alternativ — C# snippet (LINQPad / dotnet script):

```csharp
using BCrypt.Net;
var hash = BCrypt.Net.BCrypt.HashPassword("SizinŞifrə123!", workFactor: 12);
Console.WriteLine(hash);
```

Onlayn: [bcrypt-generator.com](https://bcrypt-generator.com/) — work factor **12** seçin.

## Verilənlər bazasına əlavə et

```sql
INSERT INTO dbo.AdminUser (Username, PasswordHash)
VALUES (N'admin', N'<yuxarıdakı_hash>');
```

`PasswordHash` yerinə öz şifrəniz üçün yaranan hash-i qoyun.

## Giriş

Panel: `/cp/auth`  
İstifadəçi adı: `admin` (və ya INSERT-dəki ad)  
Şifrə: hash yaradarkən istifadə etdiyiniz şifrə
