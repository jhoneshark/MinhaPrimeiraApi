using AutoMapper;
using MinhaPrimeiraApi.Models;

namespace MinhaPrimeiraApi.DTOs.Mappings;

public class CategoriesDTOMappingProfile : Profile
{
    public CategoriesDTOMappingProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
    }
}