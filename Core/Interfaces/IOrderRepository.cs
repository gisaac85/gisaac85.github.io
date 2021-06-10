using Core.Entities.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrderRepository
    {      
        Task<IReadOnlyList<Order>> GetOrdersAsync();       
        Task<Order> CreateOrder(Order model);
        Task<Order> GetOrder(int id,string email);
        Task<IReadOnlyList<Order>> GetOrdersForUser(string email);
        Task<Order> EditOrder(Order model);
        Task<Order> DeleteOrder(int id);
        Task<DeliveryMethod> GetDeliveryMethod(int id);
        Task<Order> GetOrderByPaymentIntentId(string id);
        Task<Order> UpdateOrderById(Order order);
    }
}
