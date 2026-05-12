using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Banan
{
    public int SoBan { get; set; }

    public string? TrangThai { get; set; }

    public int? Tang { get; set; }

    public int? SoGhe { get; set; }

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual ICollection<Thongbao> Thongbaos { get; set; } = new List<Thongbao>();
}
