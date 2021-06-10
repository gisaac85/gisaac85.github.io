using Core.Entities.OrderModels;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly WebshopDataContext _context;
        public DeliveryRepository(WebshopDataContext context)
        {
            _context = context;
        }
        public async Task<DeliveryMethod> CreateDeliveryMethod(DeliveryMethod model)
        {
            _context.DeliveryMethods.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<DeliveryMethod> DeleteDeliveryMethod(int id)
        {
            var delivery = _context.DeliveryMethods.FindAsync(id).Result;
            _context.DeliveryMethods.Remove(delivery);
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task<DeliveryMethod> EditDeliveryMethod(DeliveryMethod model)
        {
            _context.DeliveryMethods.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<DeliveryMethod> GetDeliveryMethod(int id)
        {
            return await _context.DeliveryMethods.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _context.DeliveryMethods.ToListAsync();
        }
    }
}
