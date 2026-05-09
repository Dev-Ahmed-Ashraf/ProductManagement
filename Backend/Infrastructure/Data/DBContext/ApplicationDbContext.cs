using DBS_Task.Domain.Entities;
using DBS_Task.Infrastructure.Data.Interceptors;
using DBS_Task.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Infrastructure.Data.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStatusHistory> ProductStatusHistories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserWithRoleView> UserWithRoles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method first to ensure Identity configurations are applied
            base.OnModelCreating(modelBuilder);

            // Apply configurations first
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Apply the soft delete query filter
            modelBuilder.ApplySoftDeleteQueryFilter();

            modelBuilder.Entity<UserWithRoleView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_UserWithRoles");
            });
        }
    }
}
