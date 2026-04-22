using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBS_Task.Migrations
{
    /// <inheritdoc />
    public partial class User_Roles_View_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW vw_UserWithRoles AS
                                                                    SELECT 
                                                                        u.Id,
                                                                        u.FullName,
                                                                        u.UserName,
                                                                        u.Email,
                                                                        u.IsActive,
                                                                        u.CreatedAt,
                                                                        r.Name AS Role
                                                                    FROM AspNetUsers u
                                                                    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                                                                    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vw_UserWithRoles");
        }
    }
}
