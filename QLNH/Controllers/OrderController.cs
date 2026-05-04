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
                int maNVHienTai = 3;
                await _orderService.XuLyDatMonAsync(request, maNVHienTai);
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

        [HttpGet("get-notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var notis = await _orderService.GetThongBaoChuaXemAsync();
            return Ok(notis);
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