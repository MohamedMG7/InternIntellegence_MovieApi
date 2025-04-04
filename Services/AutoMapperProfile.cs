using AutoMapper;
using InternIntellegence_MovieApi.DTO.MovieDtos;
using InternIntellegence_MovieApi.Models;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MovieAddDto, Movie>();
        CreateMap<MovieUpdateDto,Movie>();
        CreateMap<Movie,MovieReadDto>();
    }
}
