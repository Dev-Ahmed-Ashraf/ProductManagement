using DBS_Task.Application.Common;
using DBS_Task.Domain.Entities;
using DBS_Task.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly AuditInterceptor _auditInterceptor;

        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, AuditInterceptor auditInterceptor) : base(options)
        {
            _auditInterceptor = auditInterceptor;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations first
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Apply the soft delete query filter
            modelBuilder.ApplySoftDeleteQueryFilter();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
        }   
    }
}
