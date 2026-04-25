using DBS_Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBS_Task.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");


            // Primary Key
            builder.HasKey(x => x.Id);


            // Secure token value
            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(500);


            // Dates
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();


            // One User -> Many Refresh Tokens
            builder.HasOne(x => x.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Unique refresh token
            builder.HasIndex(x => x.Token)
                .IsUnique();


            // Useful lookup indexes
            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.ExpiresAt);
        }
    }
}
