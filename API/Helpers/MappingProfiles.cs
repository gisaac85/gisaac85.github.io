using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dtos;
using Core.Entities.ProductModels;
using Core.Entities.UserModels;
using Core.Entities.BasketModels;
using Core.Entities.OrderModels;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name));

            CreateMap<AddressUser, AddressUserDto>().ReverseMap();       
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<CustomerBasket, CustomerBasketDto>();
            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<BasketItem, BasketItemDto>();
            CreateMap<AddressDto, Address>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
               .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
               .ForMember(d => d.ShippingPrice, o => o.MapFrom(s => s.DeliveryMethod.Price));

            CreateMap<Order, OrderToUpdateDto>()
              .ForMember(d => d.DeliveryMethodId, o => o.MapFrom(s => s.DeliveryMethod.Id));

            //CreateMap<OrderToUpdateDto, Order>()
            // .ForMember(d => d.DeliveryMethod.Id, o => o.MapFrom(s => s.DeliveryMethodId));

            CreateMap<OrderDto, Order>(); 
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl));                

        }
    }
}
