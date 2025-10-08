using AutoMapper;
using LibraryManagementSystem.DTO.MembersDTO;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Profiles
{
    public class MembersProfile : Profile
    {
        public MembersProfile()
        {
            CreateMap<Member, MembersRead>();
            CreateMap<Member, MembersReadByID>();
            CreateMap<MembersPost, Member>();
            CreateMap<MembersPut, Member>();
        }
    }
}