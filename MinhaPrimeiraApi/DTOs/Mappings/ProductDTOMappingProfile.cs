using AutoMapper;
using MinhaPrimeiraApi.Models;

namespace MinhaPrimeiraApi.DTOs.Mappings;

public class ProductDTOMappingProfile : Profile
{
    public ProductDTOMappingProfile()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();
    }
}