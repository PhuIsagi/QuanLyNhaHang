using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNH.Migrations
{
    public partial class AddCustomSqlObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            //view
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_DanhSachMonBep;");
            migrationBuilder.Sql(@"
                CREATE VIEW vw_DanhSachMonBep AS
                SELECT 
                    cp.ID AS MaChiTiet, 
                    m.TenMon, 
                    cp.SoLuong, 
                    h.SoBan, 
                    cp.GhiChu, 
                    cp.TrangThai, 
                    p.ThoiGianTao
                FROM chitietphieugoi cp
                JOIN phieugoi p ON cp.MaPhieu = p.MaPhieu
                JOIN hoadon h ON p.MaHoaDon = h.MaHoaDon
                JOIN monan m ON cp.MaMon = m.MaMon
                WHERE cp.TrangThai IN ('ChoCheBien', 'DangCheBien');
            ");

            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Top10MonBanChay;");
            migrationBuilder.Sql(@"
                CREATE VIEW vw_Top10MonBanChay AS
                SELECT TOP 10
                    m.TenMon,
                    SUM(ct.SoLuong) AS TongSoLuongBan
                FROM Chitiethoadon ct
                JOIN Hoadon h ON ct.MaHoaDon = h.MaHoaDon
                JOIN Monan m ON ct.MaMon = m.MaMon
                WHERE h.TrangThai = 'DaThanhToan'
                GROUP BY m.TenMon
                ORDER BY TongSoLuongBan DESC;
            ");

            //function
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_TinhPhutCho;");
            migrationBuilder.Sql(@"
                CREATE FUNCTION fn_TinhPhutCho (@ThoiGianGoi DATETIME)
                RETURNS INT
                AS
                BEGIN
                    RETURN DATEDIFF(MINUTE, @ThoiGianGoi, GETDATE());
                END
            ");


            // store proc
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DoanhThuTheoNhom;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_DoanhThuTheoNhom
                    @TuNgay DATETIME,
                    @DenNgay DATETIME
                AS
                BEGIN
                    SELECT 
                        n.TenNhom, 
                        ISNULL(SUM(ct.SoLuong * ct.DonGia), 0) AS DoanhThu
                    FROM Chitiethoadon ct
                    JOIN Hoadon h ON ct.MaHoaDon = h.MaHoaDon
                    JOIN Monan m ON ct.MaMon = m.MaMon
                    JOIN Nhommon n ON m.MaNhom = n.MaNhom
                    WHERE h.TrangThai = 'DaThanhToan' 
                      AND h.ThoiGianRa >= @TuNgay 
                      AND h.ThoiGianRa <= @DenNgay
                    GROUP BY n.TenNhom;
                END
            ");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ThongKeDoanhThu;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ThongKeDoanhThu
                    @TuNgay DATETIME,
                    @DenNgay DATETIME
                AS
                BEGIN
                    SELECT 
                        ISNULL(SUM(TongThanhToan), 0) AS TongDoanhThu,
                        COUNT(MaHoaDon) AS TongSoHoaDon
                    FROM Hoadon
                    WHERE TrangThai = 'DaThanhToan' 
                      AND ThoiGianRa >= @TuNgay 
                      AND ThoiGianRa <= @DenNgay;
                END
            ");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_TaoDonHang;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_TaoDonHang
                    @SoBan INT,
                    @MaNV INT,
                    @JsonDanhSachMon NVARCHAR(MAX)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRAN;

                        DECLARE @MaHoaDon INT;
                        SELECT @MaHoaDon = MaHoaDon FROM hoadon WHERE SoBan = @SoBan AND TrangThai = 'ChuaThanhToan';

                        IF @MaHoaDon IS NULL
                        BEGIN
                            INSERT INTO hoadon (SoBan, MaNV_PhucVu, ThoiGianVao, TrangThai)
                            VALUES (@SoBan, @MaNV, GETDATE(), 'ChuaThanhToan');
                            SET @MaHoaDon = SCOPE_IDENTITY(); 

                            UPDATE banan SET TrangThai = 'CoKhach' WHERE SoBan = @SoBan;
                        END

                        DECLARE @MaPhieu INT;
                        INSERT INTO phieugoi (MaHoaDon, ThoiGianTao) VALUES (@MaHoaDon, GETDATE());
                        SET @MaPhieu = SCOPE_IDENTITY();

                        INSERT INTO chitietphieugoi (MaPhieu, MaMon, SoLuong, GhiChu, TrangThai)
                        SELECT @MaPhieu, JSON_VALUE(value, '$.id'), JSON_VALUE(value, '$.quantity'), JSON_VALUE(value, '$.note'), 'ChoCheBien'
                        FROM OPENJSON(@JsonDanhSachMon);

                        MERGE chitiethoadon AS target
                        USING (
                            SELECT @MaHoaDon AS MaHoaDon,
                                   JSON_VALUE(value, '$.id') AS MaMon,
                                   CAST(JSON_VALUE(value, '$.quantity') AS INT) AS SoLuong,
                                   CAST(JSON_VALUE(value, '$.price') AS DECIMAL(10,0)) AS DonGia,
                                   JSON_VALUE(value, '$.note') AS GhiChu
                            FROM OPENJSON(@JsonDanhSachMon)
                        ) AS source
                        ON (target.MaHoaDon = source.MaHoaDon AND target.MaMon = source.MaMon)
                        WHEN MATCHED THEN
                            UPDATE SET target.SoLuong = target.SoLuong + source.SoLuong
                        WHEN NOT MATCHED THEN
                            INSERT (MaHoaDon, MaMon, SoLuong, DonGia, GhiChu, TrangThaiMon, ThoiGianGoi)
                            VALUES (source.MaHoaDon, source.MaMon, source.SoLuong, source.DonGia, source.GhiChu, 'ChoCheBien', GETDATE());

                        UPDATE hoadon
                        SET TongThanhToan = (
                            SELECT COALESCE(SUM(SoLuong * DonGia), 0)
                            FROM chitiethoadon
                            WHERE MaHoaDon = @MaHoaDon
                        )
                        WHERE MaHoaDon = @MaHoaDon;

                        COMMIT TRAN;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRAN;
                        THROW; 
                    END CATCH
                END
            ");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ThanhToanHoaDon;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ThanhToanHoaDon
                    @MaHoaDon INT,
                    @TongThanhToan DECIMAL(18,2),
                    @TienKhachDua DECIMAL(18,2),
                    @GiamGia DECIMAL(18,2),
                    @PhuongThuc NVARCHAR(50)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRANSACTION;
                    BEGIN TRY
                        UPDATE Hoadon
                        SET TongThanhToan = @TongThanhToan,
                            TienKhachDua = @TienKhachDua,
                            TienThua = @TienKhachDua - @TongThanhToan,
                            GiamGia = @GiamGia,
                            TrangThai = N'DaThanhToan',
                            ThoiGianRa = GETDATE(),
                            PhuongThucThanhToan = @PhuongThuc
                        WHERE MaHoaDon = @MaHoaDon;

                        DECLARE @SoBan INT;
                        SELECT @SoBan = SoBan FROM Hoadon WHERE MaHoaDon = @MaHoaDon;
                        
                        UPDATE Banan 
                        SET TrangThai = N'Trong' 
                        WHERE SoBan = @SoBan;

                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END
            ");


            // strigger
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_ThongBaoMonXong;");
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_ThongBaoMonXong
                ON chitietphieugoi
                AFTER UPDATE
                AS
                BEGIN
                    IF UPDATE(TrangThai)
                    BEGIN
                        INSERT INTO thongbao (NoiDung, DaXem, ThoiGian)
                        SELECT 
                            N'Bàn ' + CAST(h.SoBan AS NVARCHAR(10)) + N': ' + m.TenMon + N' đã nấu xong!', 
                            0, 
                            GETDATE(),
                            h.SoBan
                        FROM inserted i 
                        JOIN deleted d ON i.ID = d.ID 
                        JOIN monan m ON i.MaMon = m.MaMon
                        JOIN phieugoi p ON i.MaPhieu = p.MaPhieu
                        JOIN hoadon h ON p.MaHoaDon = h.MaHoaDon
                        WHERE i.TrangThai = 'HoanTat' AND d.TrangThai != 'HoanTat';
                    END
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_ThongBaoMonXong;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ThanhToanHoaDon;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_TaoDonHang;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ThongKeDoanhThu;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DoanhThuTheoNhom;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_TinhPhutCho;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Top10MonBanChay;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_DanhSachMonBep;");

            migrationBuilder.DropTable(
                name: "GopYKhachHangs");
        }
    }
}