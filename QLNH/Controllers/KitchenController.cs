using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QLNH.BLL;

namespace QLNH.Controllers
{
    [Route("api")]
    [ApiController]
    public class KitchenController : ControllerBase
    {
        private readonly KitchenService _kitchenService;

        public KitchenController(KitchenService kitchenService)
        {
            _kitchenService = kitchenService;
        }

        [HttpGet("kitchen/tasks")]
        public async Task<IActionResult> GetKitchenTasks([FromQuery] string mode = "order")
        {
            var data = await _kitchenService.GetKitchenTasksAsync(mode);
            return Ok(data);
        }
    }
}