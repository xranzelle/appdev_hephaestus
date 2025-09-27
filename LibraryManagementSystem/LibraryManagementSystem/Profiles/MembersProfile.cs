using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTO.MembersDTO;

namespace LibraryManagementSystem.Profiles
{
    public class MembersProfile : Profile
    {
        public MembersProfile() 
        {
            CreateMap<Member, MembersRead>();
            CreateMap<MembersPost, Member>();
            CreateMap<MembersPut, Member>();
        }
    }
}