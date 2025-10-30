using AutoMapper;

namespace TaskForge.Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Domain.TaskItem, Domain.TaskItem>();
        }
    }
}
