using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DynamicSun.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Weather",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<string>(type: "text", nullable: false),
                    Temperature = table.Column<double>(type: "double precision", nullable: false),
                    RelativeHumidity = table.Column<double>(type: "double precision", nullable: false),
                    Td = table.Column<double>(type: "double precision", nullable: false),
                    AtmosphericPressure = table.Column<int>(type: "integer", nullable: false),
                    WindDirection = table.Column<string>(type: "text", nullable: true),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: true),
                    CloudCover = table.Column<double>(type: "double precision", nullable: true),
                    H = table.Column<double>(type: "double precision", nullable: true),
                    VV = table.Column<double>(type: "double precision", nullable: true),
                    WeatherPhenomena = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weather", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weather");
        }
    }
}
