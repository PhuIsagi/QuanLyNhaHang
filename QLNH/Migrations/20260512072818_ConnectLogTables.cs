using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNH.Migrations
{
    /// <inheritdoc />
    public partial class ConnectLogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoBan",
                table: "thongbao",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenKhachHang",
                table: "GopYKhachHang",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDungGopY",
                table: "GopYKhachHang",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayGopY",
                table: "GopYKhachHang",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "MaHoaDon",
                table: "GopYKhachHang",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_thongbao_SoBan",
                table: "thongbao",
                column: "SoBan");

            migrationBuilder.CreateIndex(
                name: "IX_GopYKhachHang_MaHoaDon",
                table: "GopYKhachHang",
                column: "MaHoaDon");

            migrationBuilder.AddForeignKey(
                name: "FK_GopYKhachHang_Hoadon",
                table: "GopYKhachHang",
                column: "MaHoaDon",
                principalTable: "hoadon",
                principalColumn: "MaHoaDon",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Thongbao_Banan",
                table: "thongbao",
                column: "SoBan",
                principalTable: "banan",
                principalColumn: "SoBan",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GopYKhachHang_Hoadon",
                table: "GopYKhachHang");

            migrationBuilder.DropForeignKey(
                name: "FK_Thongbao_Banan",
                table: "thongbao");

            migrationBuilder.DropIndex(
                name: "IX_thongbao_SoBan",
                table: "thongbao");

            migrationBuilder.DropIndex(
                name: "IX_GopYKhachHang_MaHoaDon",
                table: "GopYKhachHang");

            migrationBuilder.DropColumn(
                name: "SoBan",
                table: "thongbao");

            migrationBuilder.DropColumn(
                name: "MaHoaDon",
                table: "GopYKhachHang");

            migrationBuilder.AlterColumn<string>(
                name: "TenKhachHang",
                table: "GopYKhachHang",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDungGopY",
                table: "GopYKhachHang",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayGopY",
                table: "GopYKhachHang",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");
        }
    }
}