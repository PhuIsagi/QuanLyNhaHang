using System;
using System.ComponentModel.DataAnnotations;

namespace QLNH.Models
{
    public class GopYKhachHang
    {
        [Key]
        public int Id { get; set; }

        public string? TenKhachHang { get; set; }
        public string? NoiDungGopY { get; set; }
        public int SoSaoDanhGia { get; set; }
        public DateTime NgayGopY { get; set; }
    }
}