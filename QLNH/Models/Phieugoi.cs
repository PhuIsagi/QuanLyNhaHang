using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Phieugoi
{
    public int MaPhieu { get; set; }

    public int? MaHoaDon { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public virtual ICollection<Chitietphieugoi> Chitietphieugois { get; set; } = new List<Chitietphieugoi>();

    public virtual Hoadon? MaHoaDonNavigation { get; set; }
}
