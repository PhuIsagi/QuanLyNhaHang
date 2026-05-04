using System.Linq;
using System.Threading.Tasks;
using QLNH.DAL.Repositories;

namespace QLNH.BLL
{
    public class CheckoutRequestDto
    {
        public int ma_hoa_don { get; set; }
        public decimal tong_thanh_toan { get; set; }
        public decimal tien_khach_dua { get; set; }
        public decimal giam_gia { get; set; }
        public string? phuong_thuc { get; set; }
    }

    public class CashierService
    {
        private readonly CashierRepository _repo;

        public CashierService(CashierRepository repo)
        {
            _repo = repo;
        }

        public async Task<object> GetBillDetailsAsync(int soBan)
        {
            var hoadon = await _repo.GetActiveInvoiceByTableAsync(soBan);
            if (hoadon == null)
                return new { success = false, msg = "Bàn này hiện không có hóa đơn chưa thanh toán!" };

            var items = hoadon.Chitiethoadons.Select(ct => new {
                ten_mon = ct.MaMonNavigation?.TenMon,
                ghi_chu = ct.GhiChu,
                so_luong = ct.SoLuong,
                don_gia = ct.DonGia,
                thanh_tien = ct.SoLuong * ct.DonGia
            }).ToList();

            decimal tamTinh = items.Sum(i => i.thanh_tien ?? 0);
            decimal vat = tamTinh * 0.1m;
            decimal giamGia = hoadon.GiamGia ?? 0;
            decimal tongThanhToan = tamTinh + vat - giamGia;
            int tongSoLuong = items.Sum(i => i.so_luong ?? 0);

            return new
            {
                success = true,
                ma_hoa_don = hoadon.MaHoaDon,
                so_ban = hoadon.SoBan,
                thoi_gian_vao = hoadon.ThoiGianVao?.ToString("dd/MM/yyyy HH:mm"),
                items = items,
                calculations = new
                {
                    tong_so_luong = tongSoLuong,
                    tam_tinh = tamTinh,
                    vat = vat,
                    giam_gia = giamGia,
                    tong_thanh_toan = tongThanhToan
                }
            };
        }

        public async Task<object> ProcessCheckoutAsync(CheckoutRequestDto request)
        {
            bool result = await _repo.ThanhToanAsync(
                request.ma_hoa_don,
                request.tong_thanh_toan,
                request.tien_khach_dua,
                request.giam_gia,
                request.phuong_thuc ?? "Tiền mặt");

            if (result)
                return new { success = true, msg = "Thanh toán thành công" };

            return new { success = false, msg = "Lỗi xử lý thanh toán" };
        }
    }
}