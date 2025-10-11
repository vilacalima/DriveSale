using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Brand).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Model).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Color).IsRequired().HasMaxLength(60);
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Price).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.Status).HasConversion<short>().HasDefaultValue(VehicleStatus.Available);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => new { x.Status, x.Price });
        builder.HasIndex(x => x.Price);
    }
}

