using AutoMapper;
using TaskForge.Domain;

namespace TaskForge.Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<TaskItem, TaskItem>();
        }
    }
}
