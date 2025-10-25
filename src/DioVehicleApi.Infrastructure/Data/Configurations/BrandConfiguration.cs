using DioVehicleApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DioVehicleApi.Infrastructure.Data.Configurations;

public class BrandConfiguration : BaseEntityConfiguration<Brand, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Brands_Name");

        builder.HasMany(e => e.Models)
            .WithOne(e => e.Brand)
            .HasForeignKey(e => e.BrandId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}