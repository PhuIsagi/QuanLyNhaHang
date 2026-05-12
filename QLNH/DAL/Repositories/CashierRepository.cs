using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLNH.Models;

namespace QLNH.DAL.Repositories
{
    public class CashierRepository
    {
        private readonly AppDbContext _context;

        public CashierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Hoadon?> GetActiveInvoiceByTableAsync(int soBan)
        {
            return await _context.Hoadons
                .Include(h => h.Chitiethoadons)
                    .ThenInclude(ct => ct.MaMonNavigation)
                .FirstOrDefaultAsync(h => h.SoBan == soBan && h.TrangThai == "ChuaThanhToan");
        }

        public async Task ThanhToanAsync(int maHoaDon, decimal tongThanhToan, decimal tongTienHang, decimal tienKhachDua, decimal giamGia, decimal vat, string phuongThuc)
        {
            var pMaHoaDon = new SqlParameter("@MaHoaDon", maHoaDon);
            var pTong = new SqlParameter("@TongThanhToan", tongThanhToan);
            var pTongHang = new SqlParameter("@TongTienHang", tongTienHang);
            var pTienKhach = new SqlParameter("@TienKhachDua", tienKhachDua);
            var pGiamGia = new SqlParameter("@GiamGia", giamGia);
            var pVat = new SqlParameter("@VAT", vat);
            var pPhuongThuc = new SqlParameter("@PhuongThuc", (object)phuongThuc ?? DBNull.Value);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_ThanhToanHoaDon @MaHoaDon, @TongThanhToan, @TongTienHang, @TienKhachDua, @GiamGia, @VAT, @PhuongThuc",
                pMaHoaDon, pTong, pTongHang, pTienKhach, pGiamGia, pVat, pPhuongThuc);
        }
    }
}