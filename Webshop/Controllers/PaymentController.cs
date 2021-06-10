using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Shared;

namespace Webshop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public PaymentController(IHttpContextAccessor httpContextAccessor, IMapper mapper)
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

        public async Task<IActionResult> Index()
        {            
            await PublicMethods();
            return View();
        }

    }
}
