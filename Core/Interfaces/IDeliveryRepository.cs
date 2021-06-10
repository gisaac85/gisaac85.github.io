using Core.Entities.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDeliveryRepository
    {
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<DeliveryMethod> CreateDeliveryMethod(DeliveryMethod model);
        Task<DeliveryMethod> GetDeliveryMethod(int id);
        Task<DeliveryMethod> EditDeliveryMethod(DeliveryMethod model);
        Task<DeliveryMethod> DeleteDeliveryMethod(int id);
    }
}
