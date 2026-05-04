using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QLNH.DAL.Repositories
{
    public class KitchenRepository
    {
        private readonly AppDbContext _context;
        public KitchenRepository(AppDbContext context) => _context = context;

        public async Task<object> LayDanhSachMonBepAsync()
        {
            var result = await _context.Database.SqlQuery<KitchenDto>(
                $"SELECT MaChiTiet, TenMon, SoLuong, SoBan, GhiChu, TrangThai, ThoiGianTao FROM vw_DanhSachMonBep"
            ).ToListAsync();

            return result;
        }
    }

    public class KitchenDto
    {
        public int MaChiTiet { get; set; }
        public string? TenMon { get; set; }
        public int SoLuong { get; set; }
        public int SoBan { get; set; }
        public string? GhiChu { get; set; }
        public string? TrangThai { get; set; }
        public System.DateTime ThoiGianTao { get; set; }
    }
}