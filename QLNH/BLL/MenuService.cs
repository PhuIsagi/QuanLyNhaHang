using System.Linq;
using System.Threading.Tasks;
using QLNH.DAL.Repositories;

namespace QLNH.BLL
{
    public class MenuService
    {
        private readonly MenuRepository _repo;
        public MenuService(MenuRepository repo) => _repo = repo;

        public async Task<object> GetNhomMonAsync()
        {
            var nhom = await _repo.LayNhomMonAsync();
            return nhom.Select(n => new { maNhom = n.MaNhom, tenNhom = n.TenNhom }).ToList();
        }

        public async Task<object> GetDanhSachMonAsync()
        {
            var mon = await _repo.LayMonAnAsync();
            return mon.Select(m => new {
                maMon = m.MaMon,
                maNhom = m.MaNhom,
                tenMon = m.TenMon,
                giaTien = m.GiaTien,
                hinhAnh = m.HinhAnh
            }).ToList();
        }
    }
}