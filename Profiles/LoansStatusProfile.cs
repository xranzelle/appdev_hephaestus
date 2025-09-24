using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTO.LoanStatusDTO;

namespace LibraryManagementSystem.Profiles
{
    public class LoansStatusProfile : Profile
    {
        public LoansStatusProfile() 
        {
            CreateMap<LoanStatus, LoanStatusRead>();
        }
    }
}