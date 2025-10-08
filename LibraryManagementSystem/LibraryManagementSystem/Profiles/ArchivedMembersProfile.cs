using AutoMapper;
using LibraryManagementSystem.DTO.ArchivedMembersDTO;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Profiles
{
    public class ArchivedMembersProfile : Profile
    {
        public ArchivedMembersProfile()
        {
            CreateMap<ArchivedMember, ArchivedMembersRead>();
            CreateMap<ArchivedMember, ArchivedMembersReadByID>();
        }
    }
}