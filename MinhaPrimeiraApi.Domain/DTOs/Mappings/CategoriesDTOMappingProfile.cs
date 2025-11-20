using AutoMapper;
using MinhaPrimeiraApi.Domain.Models;

namespace MinhaPrimeiraApi.Domain.DTOs.Mappings;

public class CategoriesDTOMappingProfile : Profile
{
    public CategoriesDTOMappingProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
    }
}