using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.BasketModels;
using Core.Entities.OrderModels;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Stripe;
using Order = Core.Entities.OrderModels.Order;
using Product = Core.Entities.ProductModels.Product;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IProductRepository _productRepo;
        private readonly IDeliveryRepository _deliveryRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IConfiguration _config;

        public PaymentService(IBasketRepository basketRepository, IProductRepository productRepo, IDeliveryRepository deliveryRepo, IOrderRepository orderRepo, IConfiguration config)
        {
            _config = config;
            _basketRepository = basketRepository;
            _productRepo = productRepo;
            _deliveryRepo = deliveryRepo;
            _orderRepo = orderRepo;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId,string email)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null) return null;

            var shippingPrice = 0m;
           
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _deliveryRepo.GetDeliveryMethod((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in basket.Items)
            {
                var productItem = await _productRepo.GetProductByIdAsync(item.Id);
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();

            PaymentIntent intent;
            
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {                
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100,
                    Currency = "eur",
                    PaymentMethodTypes = new List<string> { "card" },
                    Customer = "cus_J63t1pVjvPzKF8",                
                    PaymentMethod = "pm_card_nl",
                    Confirm = true,   
                    ReceiptEmail = email,                    
                }; 
                
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long) basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long) shippingPrice * 100,
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _basketRepository.UpdateBasketAsync(basket);           
            return basket;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var order = await _orderRepo.GetOrderByPaymentIntentId(paymentIntentId);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentFailed;
            await _orderRepo.UpdateOrderById(order);
            return order;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {           
            var order = await _orderRepo.GetOrderByPaymentIntentId(paymentIntentId);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentRecevied;
            await _orderRepo.UpdateOrderById(order);          
            return order;
        }       
    }
}