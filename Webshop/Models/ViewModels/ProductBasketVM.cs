using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models.ViewModels
{
    public class ProductBasketVM
    {
        public IEnumerable<ProductToReturnDto> Products { get; set; }
        public IEnumerable<CustomerBasketDto> CustomerBasket { get; set; }
    }
}
