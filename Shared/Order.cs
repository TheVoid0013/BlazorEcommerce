using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorEcommerce.Shared
{
    [Table("Orders")]
    [Index(nameof(UserId), Name = "IX_Orders_UserId")]
    [Index(nameof(OrderDate), Name = "IX_Orders_OrderDate")]
    [Index(nameof(UserId), nameof(OrderDate), Name = "IX_Orders_User_OrderDate")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        // Navigation property
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}