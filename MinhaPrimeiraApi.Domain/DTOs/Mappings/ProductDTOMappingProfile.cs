using AutoMapper;
using MinhaPrimeiraApi.Domain.Models;

namespace MinhaPrimeiraApi.Domain.DTOs.Mappings;

public class ProductDTOMappingProfile : Profile
{
    public ProductDTOMappingProfile()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();
    }
}