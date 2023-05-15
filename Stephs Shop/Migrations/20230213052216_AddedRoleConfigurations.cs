using Microsoft.EntityFrameworkCore.Migrations;

namespace Stephs_Shop.Migrations
{
    public partial class AddedRoleConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1e68426e-da32-4c98-a766-32ba5623ca77", "5b10274e-92a0-4e97-bbf8-6c034be68824", "Member", "MEMBER" },
                    { "19caa819-3c3b-44ee-ac89-f88140fffb30", "6a4b9841-e458-41dd-bd99-2706fc959bc5", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityRole");
        }
    }
}
