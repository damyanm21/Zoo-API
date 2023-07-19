using AutoMapper;
using ZooAPI.Domain.Models.DTOs;

namespace ZooAPI.Domain.Models.Profiles
{
    public class AnimalMappingProfile : Profile
    {
        public AnimalMappingProfile()
        {
            CreateMap<Animal, AnimalDto>();
            CreateMap<AnimalDto, Animal>();
        }
    }

}
