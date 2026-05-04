using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QLNH.BLL;

namespace QLNH.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var result = await _authService.LoginAsync(req.username ?? "", req.password ?? "");
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMsg += " | Chi tiết: " + ex.InnerException.Message;
                }

                return Ok(new
                {
                    success = false,
                    message = "Lỗi C# đây Phú ơi: " + errorMsg
                });
            }
        }
    }

    public class LoginRequest
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
}