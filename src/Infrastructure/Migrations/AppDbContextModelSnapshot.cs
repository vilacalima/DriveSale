using System;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Domain.Entities.Client", b =>
            {
                b.ToTable("clients");
                b.HasKey("Id");
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Name").HasMaxLength(200).IsRequired();
                b.Property<string>("Email").HasMaxLength(200).IsRequired();
                b.Property<string>("Cpf").HasMaxLength(11).IsRequired();
                b.Property<DateTime?>("CreatedAt").HasColumnName("created_at");
                b.Property<DateTime?>("UpdatedAt").HasColumnName("updated_at");
                b.HasIndex("Cpf").IsUnique();
            });

            modelBuilder.Entity("Domain.Entities.Payment", b =>
            {
                b.ToTable("payments");
                b.HasKey("Id");
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Code").IsRequired();
                b.Property<decimal>("Amount").HasColumnType("numeric(18,2)");
                b.Property<short>("Status").HasDefaultValue((short)PaymentStatus.Pending);
                b.Property<string?>("Provider");
                b.Property<DateTime?>("CreatedAt").HasColumnName("created_at");
                b.Property<DateTime?>("UpdatedAt").HasColumnName("updated_at");
                b.HasIndex("Code").IsUnique();
            });

            modelBuilder.Entity("Domain.Entities.Vehicle", b =>
            {
                b.ToTable("vehicles");
                b.HasKey("Id");
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("Brand").HasMaxLength(120).IsRequired();
                b.Property<string>("Model").HasMaxLength(120).IsRequired();
                b.Property<int>("Year").IsRequired();
                b.Property<string>("Color").HasMaxLength(60).IsRequired();
                b.Property<decimal>("Price").HasColumnType("numeric(18,2)").IsRequired();
                b.Property<short>("Status").HasDefaultValue((short)VehicleStatus.Available);
                b.Property<DateTime?>("CreatedAt").HasColumnName("created_at");
                b.Property<DateTime?>("UpdatedAt").HasColumnName("updated_at");
                b.HasIndex("Price");
                b.HasIndex("Status", "Price");
            });

            modelBuilder.Entity("Domain.Entities.Sale", b =>
            {
                b.ToTable("sales");
                b.HasKey("Id");
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<Guid>("VehicleId");
                b.Property<string>("BuyerCpf").HasMaxLength(14).IsRequired();
                b.Property<DateTime>("SaleDate");
                b.Property<decimal>("TotalPrice").HasColumnType("numeric(18,2)").IsRequired();
                b.Property<Guid>("PaymentId");
                b.Property<bool>("Canceled").HasDefaultValue(false);
                b.Property<DateTime?>("CreatedAt").HasColumnName("created_at");
                b.Property<DateTime?>("UpdatedAt").HasColumnName("updated_at");
                b.HasIndex("PaymentId");
                b.HasIndex("VehicleId").IsUnique();
                b.HasOne("Domain.Entities.Payment", "Payment").WithMany().HasForeignKey("PaymentId").OnDelete(DeleteBehavior.Restrict);
                b.HasOne("Domain.Entities.Vehicle", "Vehicle").WithMany().HasForeignKey("VehicleId").OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

