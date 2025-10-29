using DioVehicleApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DioVehicleApi.Infrastructure.Data.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(e => e.IsAdmin)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
