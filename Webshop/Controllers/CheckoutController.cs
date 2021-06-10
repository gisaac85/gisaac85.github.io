using AutoMapper;
using Core.Dtos;
using Core.Entities.BasketModels;
using Core.Entities.OrderModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Webshop.Models.ViewModels;
using Webshop.Shared;

namespace Webshop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
      
        public CheckoutController(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Tuple<object, object, object, object>> PublicMethods()
        {           
           
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            TempData["types"] = await service.FetchProductTypes();
            TempData["brands"] = await service.FetchProducBrands();
            var basketProducts = await service.FetchBasket();
            TempData["basketItems"] = basketProducts.Items.Count;
            decimal total = 0;
            foreach (var pro in basketProducts.Items)
            {
                total = total + (pro.Price * pro.Quantity);
                TempData["total"] = total;
            }
            return Tuple.Create(TempData["types"], TempData["brands"], TempData["basketItems"], TempData["total"]);
        }

        public async Task<IActionResult> GetAllOrdersForUser()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            var basketId = _httpContextAccessor.HttpContext.Session.GetString("basketId");

            if (token == null || token == "")
            {
                TempData["NotLoggedin"] = "You must loggedIn ...";
                return RedirectToAction("Index", "Account");
            }

            var result = new List<OrderToReturnDto>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/orders/getorders"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<OrderToReturnDto>>(apiResponse);
                }
            }
            await PublicMethods();
            return View("AllOrders", result);
        }


        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, VaryByHeader = "None", NoStore = true)]
        public async Task<IActionResult> CheckoutBasket()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            var basketId = _httpContextAccessor.HttpContext.Session.GetString("basketId");
            var email = _httpContextAccessor.HttpContext.Session.GetString("Email");

            if (token == null || token == "")
            {
                TempData["NotLoggedin"] = "You must loggedIn ...";
                return RedirectToAction("Index", "Account");
            }

            Address address = new Address();
            var deliveryMethods = new List<DeliveryMethod>();
            var myModel = new Basket_Order_vm();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/account/address"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    address = JsonConvert.DeserializeObject<Address>(apiResponse);
                }
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/orders/deliverymethods"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    deliveryMethods = JsonConvert.DeserializeObject<List<DeliveryMethod>>(apiResponse);
                    ViewData["methods"] = deliveryMethods;
                }
            }

            var service = new SharedSpace(_httpContextAccessor,_mapper);
            var myBasket = await service.FetchBasket();

            myModel.customerBasketDto = _mapper.Map<CustomerBasket, CustomerBasketDto>(myBasket);
            myModel.OrderToReturnDto.ShipToAddress = new Address(address.FirstName, address.LastName, address.Street, address.City, address.State, address.Zipcode);
            myModel.OrderToReturnDto.DeliveryMethod = myBasket.DeliveryMethodId.ToString();
            myModel.OrderToReturnDto.OrderDate = DateTimeOffset.Now;
            myModel.OrderToReturnDto.Status = OrderStatus.Pending.ToString();
            myModel.OrderToReturnDto.BuyerEmail = email;

            decimal subTotal = 0;

            foreach (var item in myBasket.Items)
            {
                subTotal += item.Price * item.Quantity;
            }
            myModel.OrderToReturnDto.Subtotal = subTotal;
            myModel.OrderToReturnDto.ShippingPrice = 0;
            myModel.OrderToReturnDto.Total = subTotal + myModel.OrderToReturnDto.ShippingPrice;

            await PublicMethods();
            return View("Index", myModel);
        }


        [HttpPost]       
        [ValidateAntiForgeryToken]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, VaryByHeader = "None", NoStore = true)]
        public async Task<IActionResult> CreateOrderMVC(OrderDto input)
        {
            _httpContextAccessor.HttpContext.Session.SetString("deliveryId",input.DeliveryMethodId.ToString());
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            var createdModel = await service.CreateOrderMVC(input);
            var model = _mapper.Map<Order, OrderToReturnDto>(createdModel);         
            model.Total = createdModel.GetTotal();
            await PublicMethods();
            if (createdModel.Id == 0)
            {
                return Redirect("https://localhost:44325/");
            }
           
            return View("Payment", model);          
        }
    }
}
