using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    if (!string.IsNullOrWhiteSpace(cs))
    {
        options.UseSqlServer(cs);
        return;
    }

    options.UseInMemoryDatabase("Tortcu");
});

builder.Services.AddScoped<ISeoMetaService, SeoMetaService>();
builder.Services.AddScoped<ISlugService, SlugService>();
builder.Services.AddScoped<ISitemapService, SitemapService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

