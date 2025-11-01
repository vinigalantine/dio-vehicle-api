using DioVehicleApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DioVehicleApi.Infrastructure.Data.Configurations;

public class VehicleConfiguration : BaseEntityConfiguration<Vehicle, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        
        builder.Property(e => e.ModelId)
            .IsRequired();
        
        builder.Property(e => e.Color)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.LicensePlate)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(e => e.Year)
            .IsRequired();
        
        builder.HasOne(e => e.Model)
            .WithMany(e => e.Vehicles)
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(e => e.LicensePlate)
               .IsUnique()
               .HasFilter("[LicensePlate] IS NOT NULL AND [IsDeleted] = 0");
        
    }
}