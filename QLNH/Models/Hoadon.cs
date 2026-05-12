using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Hoadon
{
    public int MaHoaDon { get; set; }

    public int? SoBan { get; set; }

    public int? MaNvPhucVu { get; set; }

    public DateTime? ThoiGianVao { get; set; }

    public DateTime? ThoiGianRa { get; set; }

    public decimal? TongThanhToan { get; set; }

    public decimal? TongTienHang { get; set; }

    public decimal? GiamGia { get; set; }

    public decimal? Vat { get; set; }

    public string? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public decimal? TienKhachDua { get; set; }

    public decimal? TienThua { get; set; }

    public virtual ICollection<Chitiethoadon> Chitiethoadons { get; set; } = new List<Chitiethoadon>();

    public virtual Nhanvien? MaNvPhucVuNavigation { get; set; }

    public virtual ICollection<Phieugoi> Phieugois { get; set; } = new List<Phieugoi>();

    public virtual Banan? SoBanNavigation { get; set; }

    public virtual ICollection<GopYKhachHang> GopYKhachHangs { get; set; } = new List<GopYKhachHang>();
}
