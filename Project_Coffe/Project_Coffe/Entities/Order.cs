using System.ComponentModel.DataAnnotations;

namespace Project_Coffe.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderProduct>? OrderProducts { get; set; }
    }

}
