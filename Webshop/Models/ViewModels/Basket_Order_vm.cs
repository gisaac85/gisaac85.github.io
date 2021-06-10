using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models.ViewModels
{
    public class Basket_Order_vm
    {
        public CustomerBasketDto customerBasketDto { get; set; } = new CustomerBasketDto();
        public OrderToReturnDto OrderToReturnDto { get; set; } = new OrderToReturnDto();
    }
}
