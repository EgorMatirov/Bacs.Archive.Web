using Microsoft.EntityFrameworkCore.Migrations;

namespace Bacs.Archive.Web.Data.Migrations
{
    public partial class AddRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_AspNetUserRoles_UserId",
                "AspNetUserRoles");

            migrationBuilder.DropIndex(
                "RoleNameIndex",
                "AspNetRoles");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "RoleNameIndex",
                "AspNetRoles");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_UserId",
                "AspNetUserRoles",
                "UserId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName");
        }
    }
}
