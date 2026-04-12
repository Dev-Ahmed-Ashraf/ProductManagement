using DBS_Task.Application.Interfaces;
using DBS_Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DBS_Task.Application.Common
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, 
            InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAudit(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.Entity is not SoftDeleteEntity entity)
                    continue;

                // Added
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(AuditableEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(AuditableEntity.CreatedBy)).CurrentValue = _currentUserService.UserName;
                }

                // Modified
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(AuditableEntity.UpdatedBy)).CurrentValue = _currentUserService.UserName;
                }

                // Soft Delete
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;

                    entry.Property(nameof(SoftDeleteEntity.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(SoftDeleteEntity.DeletedAt)).CurrentValue = DateTime.UtcNow;
                }
            }
        }

    }
}
