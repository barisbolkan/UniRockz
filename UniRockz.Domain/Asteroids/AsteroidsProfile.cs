using AutoMapper;
using UniRockz.Domain.Asteroids.Responses;
using UniRockz.Repository.Asteroids.Entities;

namespace UniRockz.Domain.Asteroids
{
    public class AsteroidsProfile : Profile
    {
        public AsteroidsProfile()
        {
            CreateMap<AsteroidInfo, AsteroidInfoResponse>()
                .ForMember(air => air.Id, opt => opt.MapFrom(_ => _.Id))
                .ForMember(air => air.Name, opt => opt.MapFrom(_ => _.Name))
                .ForMember(air => air.JplUrl, opt => opt.MapFrom(_ => _.JplUrl))
                .ForMember(air => air.Magnitude, opt => opt.MapFrom(_ => _.AbsoluteMagnitude.Value))
                .ForMember(air => air.IsHazardous, opt => opt.MapFrom(_ => _.IsHazardous))
                .ReverseMap();
        }
    }
}
