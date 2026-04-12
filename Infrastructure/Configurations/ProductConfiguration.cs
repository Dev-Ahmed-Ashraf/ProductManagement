using DBS_Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBS_Task.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Id (Identity)
            builder.Property(p => p.Id)
                   .ValueGeneratedOnAdd();

            // Name
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            // Description
            builder.Property(p => p.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            // Price
            builder.Property(p => p.Price)
                   .HasPrecision(18, 2)
                   .IsRequired();

            // Quantity
            builder.Property(p => p.Quantity)
                   .IsRequired();

            // CreatedAt
            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            // CreatedBy
            builder.Property(p => p.CreatedBy)
                   .HasMaxLength(100)
                   .IsRequired();

            // UpdatedAt
            builder.Property(p => p.UpdatedAt)
                   .IsRequired(false);

            // UpdatedBy
            builder.Property(p => p.UpdatedBy)
                   .HasMaxLength(100)
                   .IsRequired(false);

            // IsDeleted
            builder.Property(p => p.IsDeleted)
                   .IsRequired();

            // DeleteAt
            builder.Property(p => p.DeletedAt)
                   .IsRequired(false);
        }
    }
}
