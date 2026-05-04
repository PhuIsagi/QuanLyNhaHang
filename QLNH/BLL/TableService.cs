using System.Linq;
using System.Threading.Tasks;
using QLNH.DAL.Repositories;

namespace QLNH.BLL
{
    public class TableService
    {
        private readonly TableRepository _repo;
        public TableService(TableRepository repo) => _repo = repo;

        public async Task<object> GetDanhSachBanAsync()
        {
            var tables = await _repo.LayDanhSachBanAsync();

            return tables.Select(b => new {
                soBan = b.SoBan,
                soGhe = b.SoGhe,
                tang = b.Tang,
                trangThai = b.TrangThai,
                statusText = b.TrangThai == "Trong" ? "Trống" : (b.TrangThai == "CoKhach" ? "Có khách" : "Đặt trước"),
                cssClass = b.TrangThai == "Trong" ? "bg-trong" : (b.TrangThai == "CoKhach" ? "bg-cokhach" : "bg-dattruoc"),
                iconClass = b.TrangThai == "Trong" ? "fa-check-circle" : "fa-users"
            }).ToList();
        }
    }
}