using Core.Entities.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId, Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id,string email);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<Order> UpdateOrder(Order input,string basketId);
        Task<DeliveryMethod> GetDeliveryMethod(int id);
        Task<Order> GetOrderByPaymentIntentId(string id);
        Task<Order> UpdateOrderById(Order order);
    }
}
