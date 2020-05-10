using Microsoft.EntityFrameworkCore.Migrations;

namespace rungreenlake.web.Migrations
{
    public partial class AddRaceRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RaceRecord",
                columns: table => new
                {
                    RaceRecordID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(nullable: false),
                    RaceType = table.Column<int>(nullable: false),
                    RaceTime = table.Column<int>(nullable: false),
                    MileTime = table.Column<int>(nullable: false),
                    RunGreenLakeUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceRecord", x => x.RaceRecordID);
                    table.ForeignKey(
                        name: "FK_RaceRecord_AspNetUsers_RunGreenLakeUserId",
                        column: x => x.RunGreenLakeUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaceRecord_RunGreenLakeUserId",
                table: "RaceRecord",
                column: "RunGreenLakeUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceRecord");
        }
    }
}
