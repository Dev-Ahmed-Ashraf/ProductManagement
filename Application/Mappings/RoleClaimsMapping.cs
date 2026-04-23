using DBS_Task.Application.Common.Constants;

namespace DBS_Task.Application.Mappings
{
    /// This class defines the mapping between application roles and their associated claims.
    public static class RoleClaimsMapping
    {
        // This mapping defines which claims are associated with each role in the application.
        public static Dictionary<string, List<string>> Mapping =>
        new()
        {
            {
                AppRoles.ProjectManager,
                new List<string>
                {
                    AppClaims.ProductsView,
                    AppClaims.ProductsCreate,
                    AppClaims.ProductsDelete,
                    AppClaims.ProductsChangeStatus,
                    AppClaims.ProductStatusHistoriesView,
                    AppClaims.UsersView,
                    AppClaims.UsersCreate,
                    AppClaims.StatisticsView
                }
            },
            {
                
                AppRoles.Supervisor,
                new List<string>
                {
                    AppClaims.ProductsView,
                    AppClaims.ProductsCreate,
                    AppClaims.ProductsChangeStatus,
                    //AppClaims.ProductStatusHistoriesView,
                    AppClaims.StatisticsView
                }
            },
            {
                AppRoles.WarehouseManager,
                new List<string>
                {
                    AppClaims.ProductsView,
                    AppClaims.ProductsCreate,
                    AppClaims.ProductsChangeStatus,
                    AppClaims.ProductStatusHistoriesView

                }
            }
        };
    }
}
