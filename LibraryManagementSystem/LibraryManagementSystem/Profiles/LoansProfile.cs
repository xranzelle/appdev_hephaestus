using AutoMapper;
using LibraryManagementSystem.DTO.LoansDTO;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Profiles
{
    public class LoansProfile : Profile
    {
        public LoansProfile()
        {
            CreateMap<Loan, LoansRead>();
            CreateMap<Loan, LoansReadByID>();
            CreateMap<Loan, LoansByStatusIDRead>();
            CreateMap<Loan, LoansByMemberIDRead>();
            CreateMap<LoansPut, Loan>();
            CreateMap<LoansPost, Loan>();
        }
    }
}