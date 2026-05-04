using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QLNH.BLL;

namespace QLNH.Controllers
{
    [Route("api")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;

        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _menuService.GetNhomMonAsync();
            return Ok(categories);
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetMenu()
        {
            var menu = await _menuService.GetDanhSachMonAsync();
            return Ok(menu);
        }
    }
}