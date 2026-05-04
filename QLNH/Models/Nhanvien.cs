using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Nhanvien
{
    public int MaNv { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
}
