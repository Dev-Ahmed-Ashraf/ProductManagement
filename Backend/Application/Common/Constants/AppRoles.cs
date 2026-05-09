namespace DBS_Task.Application.Common.Constants
{
    public static class AppRoles
    {
        public const string ProjectManager = "ProjectManager";
        public const string Supervisor = "Supervisor";
        public const string WarehouseManager = "WarehouseManager";

        public static readonly string[] AllRoles = 
        {
            ProjectManager,
            Supervisor,
            WarehouseManager
        };
    }
}
