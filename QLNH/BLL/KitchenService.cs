using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLNH.DAL.Repositories;

namespace QLNH.BLL
{
    public class KitchenService
    {
        private readonly KitchenRepository _repo;
        public KitchenService(KitchenRepository repo) => _repo = repo;

        public async Task<object> GetKitchenTasksAsync(string mode)
        {
            var allTasks = (List<KitchenDto>)await _repo.LayDanhSachMonBepAsync();

            if (mode == "dish")
            {
                var waiting = allTasks.Where(x => x.TrangThai == "ChoCheBien")
                    .GroupBy(x => x.TenMon)
                    .Select(g => new {
                        tenMon = g.Key,
                        soLuong = g.Sum(x => x.SoLuong),
                        ids = g.Select(x => x.MaChiTiet).ToList(),
                        ghiChuList = g.Where(x => !string.IsNullOrEmpty(x.GhiChu)).Select(x => $"Bàn {x.SoBan}: {x.GhiChu}").ToList()
                    }).ToList();

                var cooking = allTasks.Where(x => x.TrangThai == "DangCheBien")
                    .GroupBy(x => x.TenMon)
                    .Select(g => new {
                        tenMon = g.Key,
                        soLuong = g.Sum(x => x.SoLuong),
                        ids = g.Select(x => x.MaChiTiet).ToList(),
                        ghiChuList = g.Where(x => !string.IsNullOrEmpty(x.GhiChu)).Select(x => $"Bàn {x.SoBan}: {x.GhiChu}").ToList()
                    }).ToList();

                return new { waiting_list = waiting, cooking_list = cooking };
            }
            else
            {
                return new
                {
                    waiting_list = allTasks.Where(x => x.TrangThai == "ChoCheBien").ToList(),
                    cooking_list = allTasks.Where(x => x.TrangThai == "DangCheBien").ToList()
                };
            }
        }
    }
}