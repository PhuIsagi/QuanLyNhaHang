using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNH.Migrations
{
    public partial class ThemGopY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GopYKhachHang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    TenKhachHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NoiDungGopY = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SoSaoDanhGia = table.Column<int>(type: "int", nullable: false),
                    NgayGopY = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GopYKhachHang", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GopYKhachHang");
        }
    }
}