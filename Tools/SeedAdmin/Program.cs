// Run: dotnet run -- "YourPassword"
// Usage: cd Tools/SeedAdmin && dotnet run -- "Admin123!"

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run -- \"YourPassword\"");
    return 1;
}

var hash = BCrypt.Net.BCrypt.HashPassword(args[0], workFactor: 12);
Console.WriteLine("INSERT INTO dbo.AdminUser (Username, PasswordHash)");
Console.WriteLine($"VALUES (N'admin', N'{hash}');");
return 0;
