using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QLNH.BLL;
using QLNH.Models;

namespace QLNH.Controllers
{
    [Route("api/manager")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly ManagerService _managerService;
        public ManagerController(ManagerService managerService) { _managerService = managerService; }

        [HttpGet("revenue-report")]
        public async Task<IActionResult> GetReport([FromQuery] string start_date, [FromQuery] string end_date)
        {
            if (!DateTime.TryParse(start_date, out DateTime start) || !DateTime.TryParse(end_date, out DateTime end))
                return Ok(new { success = false, msg = "Ngày không hợp lệ" });

            var result = await _managerService.GetRevenueReportAsync(start, end);
            return Ok(result);
        }

        [HttpGet("dishes")]
        public async Task<IActionResult> GetDishes() => Ok(await _managerService.GetAllDishesAsync());

        [HttpPost("dishes")]
        public async Task<IActionResult> AddDish([FromBody] Monan monan)
        {
            await _managerService.AddDishAsync(monan);
            return Ok(new { success = true });
        }

        [HttpPut("dishes/{id}")]
        public async Task<IActionResult> UpdateDish(int id, [FromBody] Monan monan)
        {
            await _managerService.UpdateDishAsync(id, monan);
            return Ok(new { success = true });
        }

        [HttpDelete("dishes/{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            await _managerService.DeleteDishAsync(id);
            return Ok(new { success = true });
        }
    }
}