using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QLNH.BLL;

namespace QLNH.Controllers
{
    [Route("api")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly TableService _tableService;

        public TableController(TableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet("tables")]
        public async Task<IActionResult> GetTables()
        {
            var tables = await _tableService.GetDanhSachBanAsync();
            return Ok(tables);
        }
    }
}