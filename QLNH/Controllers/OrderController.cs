using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QLNH.BLL;
using QLNH.Models;

namespace QLNH.Controllers
{
    [Route("api")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("luu-don-hang")]
        public async Task<IActionResult> LuuDonHang([FromBody] OrderDto request)
        {
            try
            {
                if (request.ma_nv <= 0)
                {
                    return Ok(new { success = false, msg = "Lỗi: Không tìm thấy thông tin nhân viên, vui lòng đăng nhập lại!" });
                }

                await _orderService.XuLyDatMonAsync(request, request.ma_nv);
                return Ok(new { success = true, msg = "Đã gửi bếp thành công!" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("orders/active")]
        public async Task<IActionResult> GetActiveOrders()
        {
            var orders = await _orderService.GetDanhSachDonHangActiveAsync();
            return Ok(orders);
        }

        [HttpPost("cap-nhat-mon")]
        public async Task<IActionResult> UpdateDishStatus([FromBody] UpdateDishDto request)
        {
            try
            {
                await _orderService.CapNhatTrangThaiMonAsync(request);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, msg = ex.Message });
            }
        }

        [HttpGet("thong-bao/{soBan}")]
        public async Task<IActionResult> GetThongBao(int soBan)
        {
            var result = await _orderService.GetThongBaoChuaXemAsync(soBan);
            return Ok(result);
        }

        [HttpPost("gop-y")]
        public async Task<IActionResult> NhanGopYKhachHang([FromBody] GopYKhachHang request)
        {
            if (request.MaHoaDon == null || request.SoSaoDanhGia < 1)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ!" });

            await _orderService.XuLyGopYAsync(request);
            return Ok(new { success = true, message = "Cảm ơn quý khách đã để lại đánh giá!" });
        }
    }

    public class UpdateDishDto
    {
        public string? type { get; set; }
        public int id { get; set; }
        public string? status { get; set; }
        public List<int>? ids { get; set; }
    }
}