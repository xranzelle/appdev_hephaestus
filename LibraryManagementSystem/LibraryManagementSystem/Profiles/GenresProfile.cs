using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTO.GenresDTO;

namespace LibraryManagementSystem.Profiles
{
    public class GenresProfile : Profile
    {
        public GenresProfile() 
        {
            CreateMap<Genre, GenresRead>();
            CreateMap<GenresPost, Genre>();
            CreateMap<GenresPut, Genre>();
        }
    }
}