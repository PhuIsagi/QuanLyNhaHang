using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNH.Models
{
    public class GopYKhachHang
    {
        [Key]
        public int Id { get; set; }
        public int? MaHoaDon { get; set; }
        public string? TenKhachHang { get; set; }
        public string? NoiDungGopY { get; set; }
        public int SoSaoDanhGia { get; set; }
        public DateTime NgayGopY { get; set; }

        [ForeignKey("MaHoaDon")]
        public virtual Hoadon? MaHoaDonNavigation { get; set; }
    }
}