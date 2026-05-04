using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLNH.Models;
using QLNH.Controllers;

namespace QLNH.DAL.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TaoDonHangAsync(int soBan, int maNV, string jsonDanhSachMon)
        {
            var paramSoBan = new SqlParameter("@SoBan", soBan);
            var paramMaNV = new SqlParameter("@MaNV", maNV);
            var paramJson = new SqlParameter("@JsonDanhSachMon", jsonDanhSachMon ?? "");

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_TaoDonHang @SoBan, @MaNV, @JsonDanhSachMon",
                paramSoBan, paramMaNV, paramJson);

            return true;
        }

        public async Task<object> LayDonHangActiveAsync()
        {
            var gioHienTai = DateTime.Now;

            return await _context.Hoadons
                .Where(h => h.TrangThai == "ChuaThanhToan")
                .Select(h => new {
                    maHoaDon = h.MaHoaDon,
                    soBan = h.SoBan,
                    tongThanhToan = h.TongThanhToan,

                    waitedMin = h.ThoiGianVao.HasValue
                                ? (int)(gioHienTai - h.ThoiGianVao.Value).TotalMinutes
                                : 0,

                    isCompleted = _context.Chitiethoadons
                                    .Where(c => c.MaHoaDon == h.MaHoaDon)
                                    .All(c => c.TrangThaiMon == "DaPhucVu" || c.TrangThaiMon == "HoanTat"),

                    chiTiet = _context.Chitiethoadons
                                .Where(c => c.MaHoaDon == h.MaHoaDon)
                                .Join(_context.Monans, c => c.MaMon, m => m.MaMon, (c, m) => new {
                                    maChiTiet = c.MaChiTiet,
                                    tenMon = m.TenMon,
                                    soLuong = c.SoLuong,
                                    ghiChu = c.GhiChu,
                                    trangThaiMon = c.TrangThaiMon
                                }).ToList()
                })
                .OrderByDescending(h => h.maHoaDon)
                .ToListAsync();
        }

        public async Task CapNhatTrangThaiMonAsync(UpdateDishDto req)
        {
            string safeStatus = req.status ?? "";

            if (req.type == "bep-group" && req.ids != null && req.ids.Any())
            {
                var idsStr = string.Join(",", req.ids);
                var paramStatus = new SqlParameter("@Status", safeStatus);

                string query1 = "UPDATE chitietphieugoi SET TrangThai = @Status WHERE ID IN (" + idsStr + ")";
                await _context.Database.ExecuteSqlRawAsync(query1, paramStatus);

                string query2 = @"
                    UPDATE ch SET ch.TrangThaiMon = @Status
                    FROM chitiethoadon ch
                    JOIN phieugoi p ON ch.MaHoaDon = p.MaHoaDon
                    JOIN chitietphieugoi cp ON cp.MaPhieu = p.MaPhieu AND cp.MaMon = ch.MaMon
                    WHERE cp.ID IN (" + idsStr + ")";

                await _context.Database.ExecuteSqlRawAsync(query2, new SqlParameter("@Status", safeStatus));
            }
            else if (req.type == "bep-single")
            {
                string query1 = "UPDATE chitietphieugoi SET TrangThai = @Status WHERE ID = @Id";
                await _context.Database.ExecuteSqlRawAsync(query1,
                    new SqlParameter("@Status", safeStatus),
                    new SqlParameter("@Id", req.id));

                string query2 = @"
                    UPDATE ch SET ch.TrangThaiMon = @Status
                    FROM chitiethoadon ch
                    JOIN phieugoi p ON ch.MaHoaDon = p.MaHoaDon
                    JOIN chitietphieugoi cp ON cp.MaPhieu = p.MaPhieu AND cp.MaMon = ch.MaMon
                    WHERE cp.ID = @Id";

                await _context.Database.ExecuteSqlRawAsync(query2,
                    new SqlParameter("@Status", safeStatus),
                    new SqlParameter("@Id", req.id));
            }

            else if (req.type == "group" && req.ids != null)
            {
                var items = await _context.Chitiethoadons.Where(c => req.ids.Contains(c.MaChiTiet)).ToListAsync();
                foreach (var item in items) { item.TrangThaiMon = safeStatus; }
                await _context.SaveChangesAsync();
            }
            else
            {
                var item = await _context.Chitiethoadons.FirstOrDefaultAsync(c => c.MaChiTiet == req.id);
                if (item != null) { item.TrangThaiMon = safeStatus; }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<object> LayThongBaoAsync()
        {
            return await _context.Thongbaos.Where(t => t.DaXem == false).ToListAsync();
        }
    }
}