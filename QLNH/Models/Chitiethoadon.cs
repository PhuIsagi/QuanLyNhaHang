using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Chitiethoadon
{
    public int MaChiTiet { get; set; }

    public int? MaHoaDon { get; set; }

    public int? MaMon { get; set; }

    public int? SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public string? TrangThaiMon { get; set; }

    public DateTime? ThoiGianGoi { get; set; }

    public string? GhiChu { get; set; }

    public virtual Hoadon? MaHoaDonNavigation { get; set; }

    public virtual Monan? MaMonNavigation { get; set; }
}
