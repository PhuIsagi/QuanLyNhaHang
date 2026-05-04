using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Monan
{
    public int MaMon { get; set; }

    public string? MaCode { get; set; }

    public string TenMon { get; set; } = null!;

    public string? DonVi { get; set; }

    public decimal GiaTien { get; set; }

    public int? MaNhom { get; set; }

    public string? HinhAnh { get; set; }

    public bool? DangKinhDoanh { get; set; }

    public virtual ICollection<Chitiethoadon> Chitiethoadons { get; set; } = new List<Chitiethoadon>();

    public virtual ICollection<Chitietphieugoi> Chitietphieugois { get; set; } = new List<Chitietphieugoi>();

    public virtual Nhommon? MaNhomNavigation { get; set; }
}
