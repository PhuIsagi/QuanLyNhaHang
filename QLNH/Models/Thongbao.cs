using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Thongbao
{
    public int MaTb { get; set; }

    public string NoiDung { get; set; } = null!;

    public bool? DaXem { get; set; }

    public DateTime? ThoiGian { get; set; }
}
