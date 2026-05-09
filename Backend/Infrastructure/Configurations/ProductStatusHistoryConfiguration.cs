using DBS_Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBS_Task.Infrastructure.Configurations
{
    public class ProductStatusHistoryConfiguration : IEntityTypeConfiguration<ProductStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ProductStatusHistory> builder)
        {
            builder.ToTable("ProductStatusHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.OldStatus)
               .IsRequired();

            builder.Property(x => x.NewStatus)
               .IsRequired();

            builder.Property(x => x.ProductId)
               .IsRequired();

            // CreatedAt
            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            // CreatedBy
            builder.Property(p => p.CreatedBy)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => new { x.ProductId, x.CreatedAt });

            builder.HasOne(h => h.Product)
                .WithMany(p => p.StatusHistories)
                .HasForeignKey(h => h.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
