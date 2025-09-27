using AutoMapper;
using LibraryManagementSystem.DTO.ArchiveMembersDTO;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Profiles
{
    public class ArchivedMembersProfile : Profile
    {
        public ArchivedMembersProfile()
        {
            CreateMap<ArchivedMember, ArchivedMembersRead>();
        }
    }
}