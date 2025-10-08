using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTO.BooksDTO;

namespace LibraryManagementSystem.Profiles
{
    public class BooksProfile : Profile
    {
        public BooksProfile() 
        {
            CreateMap<Book, BooksRead>();
            CreateMap<Book, BooksReadByID>();
            CreateMap<Book, BooksByAuthorIDRead>();
            CreateMap<BooksPost, Book>();
            CreateMap<BooksPut, Book>();
        }   
    }
}