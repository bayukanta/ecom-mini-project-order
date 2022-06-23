using AutoMapper;
using API.DTO;
using DAL.Models;


namespace API
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<OrderDTO, Order>();
            CreateMap<Order, OrderDTO>();
            CreateMap<ProductDTO, Product>();
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductSearchDTO, Product>();
            CreateMap<Product, ProductSearchDTO>();
            CreateMap<OrderDetailDTO, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailDTO>();
            CreateMap<OrderDetailWithDataDTO, OrderDetail>()
                .ForMember(s => s.ProductId, d => d.MapFrom(t => t.Product.Id))
                .ForMember(s => s.Product, opt => opt.Ignore());
            CreateMap<OrderDetail, OrderDetailWithDataDTO>();
        }
    }
}
