using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Chitietphieugoi
{
    public int Id { get; set; }

    public int? MaPhieu { get; set; }

    public int? MaMon { get; set; }

    public int SoLuong { get; set; }

    public string? GhiChu { get; set; }

    public string? TrangThai { get; set; }

    public virtual Monan? MaMonNavigation { get; set; }

    public virtual Phieugoi? MaPhieuNavigation { get; set; }
}
