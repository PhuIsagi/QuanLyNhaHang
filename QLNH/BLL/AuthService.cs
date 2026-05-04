using System.Threading.Tasks;
using QLNH.DAL.Repositories;

namespace QLNH.BLL
{
    public class AuthService
    {
        private readonly AuthRepository _repo;

        public AuthService(AuthRepository repo)
        {
            _repo = repo;
        }

        public async Task<object> LoginAsync(string username, string password)
        {
            var user = await _repo.KiemTraDangNhapAsync(username, password);

            if (user == null)
            {
                return new { success = false, message = "Tài khoản hoặc mật khẩu không chính xác!" };
            }

            string redirectUrl = "index.html";
            if (user.VaiTro == "PhucVu") redirectUrl = "layout/table_list.html";
            else if (user.VaiTro == "Bep") redirectUrl = "layout/kitchen.html";
            else if (user.VaiTro == "ThuNgan") redirectUrl = "layout/cashier.html";
            else if (user.VaiTro == "QuanLy") redirectUrl = "layout/manager.html";

            return new
            {
                success = true,
                message = "Đăng nhập thành công!",
                redirectUrl = redirectUrl,
                fullName = user.HoTen,
                role = user.VaiTro,
                maNV = user.MaNv,
                avatar = user.Avatar
            };
        }
    }
}