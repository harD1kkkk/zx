using Project_Coffe.Entities;

namespace Project_Coffe.Models.ModelInterface
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order> GetOrderById(int id);
        Task<Order> CreateOrder(Order order, List<int> productIds, List<int> quantities);
        Task<bool> DeleteOrder(int id);
    }
}
