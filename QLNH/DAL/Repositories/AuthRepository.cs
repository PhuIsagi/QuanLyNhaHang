using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLNH.Models;

namespace QLNH.DAL.Repositories
{
    public class AuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Nhanvien?> KiemTraDangNhapAsync(string username, string password)
        {
            return await _context.Nhanviens
                .FirstOrDefaultAsync(nv => nv.TenDangNhap == username && nv.MatKhau == password);
        }
    }
}