using Microsoft.EntityFrameworkCore;
using Tortcu.Domain;

namespace Tortcu.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<AboutContent> AboutContents => Set<AboutContent>();
    public DbSet<SeoMeta> SeoMetas => Set<SeoMeta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("Category");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Slug).HasMaxLength(220).IsRequired();
            b.Property(x => x.IsActive).HasDefaultValue(true);
            b.HasIndex(x => x.Slug).IsUnique();
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("Product");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(240).IsRequired();
            b.Property(x => x.Slug).HasMaxLength(260).IsRequired();
            b.Property(x => x.Description).HasMaxLength(4000);
            b.Property(x => x.Price).HasColumnType("decimal(18,2)");
            b.Property(x => x.IsActive).HasDefaultValue(true);
            b.Property(x => x.IsPopular).HasDefaultValue(false);
            b.Property(x => x.CreatedAtUtc).HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            b.HasIndex(x => x.Slug).IsUnique();
            b.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<ProductImage>(b =>
        {
            b.ToTable("ProductImage");
            b.HasKey(x => x.Id);
            b.Property(x => x.ImageUrl).HasMaxLength(600).IsRequired();
            b.Property(x => x.IsPrimary).HasDefaultValue(false);
            b.HasOne(x => x.Product)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId);
        });

        modelBuilder.Entity<Campaign>(b =>
        {
            b.ToTable("Campaign");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.SubTitle).HasMaxLength(400);
            b.Property(x => x.ImageUrl).HasMaxLength(600);
            b.Property(x => x.IsActive).HasDefaultValue(false);
        });

        modelBuilder.Entity<AboutContent>(b =>
        {
            b.ToTable("AboutContent");
            b.HasKey(x => x.Id);
            b.Property(x => x.Content).HasMaxLength(12000).IsRequired();
            b.Property(x => x.MainImageUrl).HasMaxLength(600);
        });

        modelBuilder.Entity<SeoMeta>(b =>
        {
            b.ToTable("SeoMeta");
            b.HasKey(x => x.Id);
            b.Property(x => x.PageType).HasMaxLength(60).IsRequired();
            b.Property(x => x.MetaTitle).HasMaxLength(300);
            b.Property(x => x.MetaDescription).HasMaxLength(600);
            b.Property(x => x.MetaKeywords).HasMaxLength(600);
            b.Property(x => x.OgTitle).HasMaxLength(300);
            b.Property(x => x.OgDescription).HasMaxLength(600);
            b.Property(x => x.OgImageUrl).HasMaxLength(600);
            b.HasIndex(x => new { x.PageType, x.EntityId }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}

