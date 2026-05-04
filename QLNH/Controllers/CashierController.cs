using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QLNH.BLL;

namespace QLNH.Controllers
{
    [Route("api")]
    [ApiController]
    public class CashierController : ControllerBase
    {
        private readonly CashierService _cashierService;

        public CashierController(CashierService cashierService)
        {
            _cashierService = cashierService;
        }

        [HttpGet("get-bill/{soBan}")]
        public async Task<IActionResult> GetBill(int soBan)
        {
            var result = await _cashierService.GetBillDetailsAsync(soBan);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            if (request == null || request.ma_hoa_don <= 0)
            {
                return BadRequest(new { success = false, msg = "Dữ liệu không hợp lệ" });
            }

            var result = await _cashierService.ProcessCheckoutAsync(request);
            return Ok(result);
        }
    }
}