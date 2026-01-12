using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddtblCarts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblCart",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TransportationId = table.Column<int>(type: "integer", nullable: false),
                    CountTickets = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCart", x => new { x.TransportationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_tblCart_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblCart_tblTransportations_TransportationId",
                        column: x => x.TransportationId,
                        principalTable: "tblTransportations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblCart_UserId",
                table: "tblCart",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblCart");
        }
    }
}
