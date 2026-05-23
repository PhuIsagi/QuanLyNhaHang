using System.Collections.Generic;

namespace QLNH.Models
{
    public class OrderItemDto
    {
        public int id { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string? note { get; set; }
    }

    public class OrderDto
    {
        public int so_ban { get; set; }
        public int ma_nv { get; set; }
        public List<OrderItemDto>? items { get; set; }
    }
}