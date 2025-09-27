using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTO.AuthorsDTO;

namespace LibraryManagementSystem.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile() 
        {
            CreateMap<Author, AuthorsRead>();
            CreateMap<AuthorsPost, Author>();
            CreateMap<AuthorsPut, Author>();
        }
    }
}