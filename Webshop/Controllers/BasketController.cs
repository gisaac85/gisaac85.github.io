using AutoMapper;
using Core.Dtos;
using Core.Entities.BasketModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Webshop.Shared;

namespace Webshop.Controllers
{
    public class BasketController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public BasketController(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {

            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Tuple<object, object, object,object>> PublicMethods()
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


        public async Task<IActionResult> Index()
        {
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            var result = await service.FetchBasket();
            await PublicMethods();           
            return View(result);
        }

        public async Task<IActionResult> AddToBasket(ProductToReturnDto product)
        {         
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            await service.AddProductToBasket(product);          
            return RedirectToAction("Index", "Products");
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            var result = await service.FetchBasket();
            var item = result.Items.Where(x => x.Id == id).FirstOrDefault();
            result.Items.Remove(item);
            var customerBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(result);
            await service.UpdateBasketMVC(customerBasket);
            await PublicMethods();            
            return View("Index",result);
        }

        public async Task<IActionResult> IncrementItemQuantity(int id)
        {
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            var result = await service.FetchBasket();
            var item = result.Items.FindIndex(x => x.Id == id);
            result.Items[item].Quantity++;
            var customerBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(result);
            await service.UpdateBasketMVC(customerBasket);
            await PublicMethods();           
            return View("Index",result);
        }

        public async Task<IActionResult> DecrementItemQuantity(int id)
        {
            var service = new SharedSpace(_httpContextAccessor,_mapper);
            var result = await service.FetchBasket();
            var item = result.Items.FindIndex(x => x.Id == id);
            if (result.Items[item].Quantity > 1)
            {
                result.Items[item].Quantity--;
                var customerBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(result);
                await service.UpdateBasketMVC(customerBasket);
                await PublicMethods();              
                return View("Index", result);
            }
            else
            {
               return await RemoveItem(id);              
            }           
        }
    }
}
