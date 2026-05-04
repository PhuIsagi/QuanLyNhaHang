using System;
using System.Collections.Generic;

namespace QLNH.Models;

public partial class Nhommon
{
    public int MaNhom { get; set; }

    public string TenNhom { get; set; } = null!;

    public virtual ICollection<Monan> Monans { get; set; } = new List<Monan>();
}
