using DBS_Task.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DBS_Task.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
        {
            var softDeleteEntities = modelBuilder.Model
                .GetEntityTypes()
                .Where(e => typeof(SoftDeleteEntity).IsAssignableFrom(e.ClrType));

            foreach (var entityType in softDeleteEntities)
            {
                var filter = CreateFilterExpression(entityType.ClrType);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

        private static LambdaExpression CreateFilterExpression(Type entityType)
        {
            // e
            var parameter = Expression.Parameter(entityType, "e");

            // e.IsDeleted
            var property = Expression.Property(parameter, nameof(SoftDeleteEntity.IsDeleted));

            // false
            var falseConstant = Expression.Constant(false);

            // e.IsDeleted == false
            var body = Expression.Equal(property, falseConstant);

            // e => e.IsDeleted == false
            return Expression.Lambda(body, parameter);
        }
    }
}
