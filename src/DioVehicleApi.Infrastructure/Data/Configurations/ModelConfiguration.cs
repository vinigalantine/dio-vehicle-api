using DioVehicleApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DioVehicleApi.Infrastructure.Data.Configurations;

public class ModelConfiguration : BaseEntityConfiguration<Model, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Model> builder)
    {
        builder.ToTable("Models");
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.BrandId)
            .IsRequired();

        builder.HasIndex(e => new { e.BrandId, e.Name })
            .IsUnique()
            .HasDatabaseName("IX_Models_BrandId_Name");

        builder.HasOne(e => e.Brand)
            .WithMany(e => e.Models)
            .HasForeignKey(e => e.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Vehicles)
            .WithOne(e => e.Model)
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}