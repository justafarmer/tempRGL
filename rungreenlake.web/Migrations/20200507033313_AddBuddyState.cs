using Microsoft.EntityFrameworkCore.Migrations;

namespace rungreenlake.web.Migrations
{
    public partial class AddBuddyState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuddyState",
                columns: table => new
                {
                    FirstID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecondID = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    FirstProfileId = table.Column<string>(nullable: true),
                    SecondProfileId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuddyState", x => x.FirstID);
                    table.ForeignKey(
                        name: "FK_BuddyState_AspNetUsers_FirstProfileId",
                        column: x => x.FirstProfileId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuddyState_AspNetUsers_SecondProfileId",
                        column: x => x.SecondProfileId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuddyState_FirstProfileId",
                table: "BuddyState",
                column: "FirstProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_BuddyState_SecondProfileId",
                table: "BuddyState",
                column: "SecondProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuddyState");
        }
    }
}
