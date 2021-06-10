using API.Errors;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using Core.Dtos;
using Core.Entities.OrderModels;
using Core.Entities.UserModels;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]  
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService; 
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        
        public OrdersController(IOrderService orderService,IMapper mapper, UserManager<AppUser> userManager)
        {
            _orderService = orderService; 
            _mapper = mapper;
            _userManager = userManager;          
        }

        [HttpPost("createOrder")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrinicipal();
            var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);
            var order = await _orderService.CreateOrderAsync(email,orderDto.DeliveryMethodId,orderDto.BasketId,address);
            if(order == null)
            {
                return BadRequest(new ApiResponse(400,"Problem in creating order"));
            }
            return Ok(order);
        }

        [HttpGet("getorders")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
        {
            var email = HttpContext.User.RetrieveEmailFromPrinicipal();
            var orders = await _orderService.GetOrdersForUserAsync(email);
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrinicipal();
            var order = await _orderService.GetOrderByIdAsync(id, email);
            if(order == null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(_mapper.Map<Order,OrderToReturnDto>(order));
        }

        [HttpGet("deliverymethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {           
            return Ok(await _orderService.GetDeliveryMethodsAsync());
        }

        [HttpGet("deliverymethod/{id}")]
        public async Task<ActionResult<DeliveryMethod>> GetDeliveryMethodById(int id)
        {
            return Ok(await _orderService.GetDeliveryMethod(id));
        }

        [HttpPost("updateOrder")]
        public async Task<ActionResult<OrderToReturnDto>> UpdateOrder(OrderToUpdateDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user.Email == input.Email)
            {
                var order = await _orderService.GetOrderByIdAsync(input.Id, input.Email);

                var address = new Address
                {
                    FirstName = input.ShipToAddress_FirstName,
                    LastName = input.ShipToAddress_LastName,
                    City = input.ShipToAddress_City,
                    Street = input.ShipToAddress_Street,
                    State = input.ShipToAddress_State,
                    Zipcode = input.ShipToAddress_Zipcode
                };

                var method = await _orderService.GetDeliveryMethod(input.DeliveryMethodId);

                var model = new Order
                {
                    BuyerEmail = input.Email,
                    Id = order.Id,
                    OrderDate = DateTimeOffset.Now,
                    ShipToAddress = address,
                    DeliveryMethod = method,
                    OrderItems = order.OrderItems,
                    PaymentIntentId = string.Empty,
                    Subtotal = order.Subtotal,
                    Status = OrderStatus.PaymentRecevied
                };
                model.GetTotal();
                var result = await _orderService.UpdateOrder(model, input.basketId);
                return Ok(_mapper.Map<Order, OrderToReturnDto>(result));
            }
            else
            {
                return Ok();
            }
        }
    }
}
