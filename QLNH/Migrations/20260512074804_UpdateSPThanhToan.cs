using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNH.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSPThanhToan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER PROCEDURE [dbo].[sp_ThanhToanHoaDon]
                    @MaHoaDon INT,
                    @TongThanhToan DECIMAL(10, 0),
                    @TongTienHang DECIMAL(10, 0),
                    @TienKhachDua DECIMAL(10, 0),
                    @GiamGia DECIMAL(10, 0),
                    @VAT DECIMAL(10, 0),
                    @PhuongThuc NVARCHAR(50)
                AS
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION;
                        
                        UPDATE Hoadon
                        SET TrangThai = 'DaThanhToan',
                            TongThanhToan = @TongThanhToan,
                            TongTienHang = @TongTienHang,
                            TienKhachDua = @TienKhachDua,
                            TienThua = @TienKhachDua - @TongThanhToan,
                            GiamGia = @GiamGia,
                            Vat = @VAT,
                            PhuongThucThanhToan = @PhuongThuc,
                            ThoiGianRa = GETDATE()
                        WHERE MaHoaDon = @MaHoaDon;

                        DECLARE @SoBan INT;
                        SELECT @SoBan = SoBan FROM Hoadon WHERE MaHoaDon = @MaHoaDon;
                        UPDATE Banan SET TrangThai = 'Trong' WHERE SoBan = @SoBan;

                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Khôi phục lại Procedure cũ
            migrationBuilder.Sql(@"
                ALTER PROCEDURE [dbo].[sp_ThanhToanHoaDon]
                    @MaHoaDon INT,
                    @TongThanhToan DECIMAL(10, 0),
                    @TienKhachDua DECIMAL(10, 0),
                    @GiamGia DECIMAL(10, 0),
                    @PhuongThuc NVARCHAR(50)
                AS
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION;
                        
                        UPDATE Hoadon
                        SET TrangThai = 'DaThanhToan',
                            TongThanhToan = @TongThanhToan,
                            TienKhachDua = @TienKhachDua,
                            TienThua = @TienKhachDua - @TongThanhToan,
                            GiamGia = @GiamGia,
                            PhuongThuc = @PhuongThuc,
                            ThoiGianRa = GETDATE()
                        WHERE MaHoaDon = @MaHoaDon;

                        DECLARE @SoBan INT;
                        SELECT @SoBan = SoBan FROM Hoadon WHERE MaHoaDon = @MaHoaDon;
                        UPDATE Banan SET TrangThai = 'Trong' WHERE SoBan = @SoBan;

                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END
            ");
        }
    }
}