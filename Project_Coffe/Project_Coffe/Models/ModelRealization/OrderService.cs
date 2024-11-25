using Microsoft.EntityFrameworkCore;
using Project_Coffe.Data;
using Project_Coffe.Entities;
using Project_Coffe.Models.ModelInterface;

namespace Project_Coffe.Models.ModelRealization
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();
        }

        public async Task<Order> GetOrderById(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order ?? throw new KeyNotFoundException("Order not found.");
        }

        public async Task<Order> CreateOrder(Order order, List<int> productIds, List<int> quantities)
        {
            if (productIds.Count != quantities.Count)
            {
                throw new ArgumentException("Product IDs and quantities count mismatch.");
            }

            decimal totalAmount = 0;
            List<OrderProduct> orderProducts = [];
            for (int i = 0; i < productIds.Count; i++)
            {
                var product = await _dbContext.Products
               .Where(p => p.Id == productIds[i])
               .FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {productIds[i]} not found.");
                }

                if (product.Stock < quantities[i])
                {
                    throw new InvalidOperationException($"Not enough stock for product {product.Name}.");
                }

                product.Stock -= quantities[i];
                totalAmount += product.Price * quantities[i];

                orderProducts.Add(new OrderProduct
                {
                    ProductId = product.Id,
                    Quantity = quantities[i]
                });
            }

            order.TotalAmount = totalAmount;
            order.OrderProducts = orderProducts;

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            foreach (var orderProduct in order.OrderProducts)
            {
                var product = await _dbContext.Products.FindAsync(orderProduct.ProductId);
                if (product != null)
                {
                    product.Stock += orderProduct.Quantity;
                }
            }

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
