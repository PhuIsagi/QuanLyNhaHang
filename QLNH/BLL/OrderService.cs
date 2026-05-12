using System.Text.Json;
using System.Threading.Tasks;
using QLNH.Models;
using QLNH.DAL.Repositories;
using QLNH.Controllers;

namespace QLNH.BLL
{
    public class OrderService
    {
        private readonly OrderRepository _repository;

        public OrderService(OrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> XuLyDatMonAsync(OrderDto orderRequest, int maNV)
        {
            string jsonItems = JsonSerializer.Serialize(orderRequest.items);



            return await _repository.TaoDonHangAsync(orderRequest.so_ban, maNV, jsonItems);
        }

        public async Task<object> GetDanhSachDonHangActiveAsync()
        {
            return await _repository.LayDonHangActiveAsync();
        }

        public async Task CapNhatTrangThaiMonAsync(UpdateDishDto request)
        {
            await _repository.CapNhatTrangThaiMonAsync(request);
        }

        public async Task CapNhatTrangThaiChiTietAsync(int id, string status)
        {
            var request = new UpdateDishDto
            {
                type = "mon-an",
                id = id,
                status = status
            };
            await _repository.CapNhatTrangThaiMonAsync(request);
        }

        public async Task XuLyGopYAsync(GopYKhachHang gopy)
        {
            gopy.NgayGopY = DateTime.Now;
            await _repository.ThemGopYKhachHangAsync(gopy);
        }

        public async Task<object> GetThongBaoChuaXemAsync(int soBan)
        {
            return await _repository.LayThongBaoAsync(soBan);
        }
    }
}