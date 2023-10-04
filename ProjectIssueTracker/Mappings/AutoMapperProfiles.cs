using AutoMapper;
using Microsoft.Build.Framework;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ProjectCreateDto, Project>().ReverseMap();
            CreateMap<UserRegistrationDto, User>().ReverseMap();
            CreateMap<UserLoginDto, User>().ReverseMap();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.CreatedIssues, opt => opt.MapFrom(src => src.CreatedIssues))
                .ForMember(dest => dest.CollaboratedProjects, opt => opt.MapFrom(src => src.CollaborativeProjects))
                .ForMember(dest=>dest.Projects, opt=>opt.MapFrom(src=>src.OwnedProjects))
                .ForMember(dest=>dest.Comments,opt=>opt.MapFrom(src=>src.Comments))
                .ReverseMap();

            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.Issues, opt => opt.MapFrom(src => src.Issues))
                .ForMember(dest => dest.Collaborators, opt => opt.MapFrom(src => src.Collaborators))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
                .ReverseMap();

            CreateMap<CollaboratorDto, ProjectCollaborator>()
                .ReverseMap();

            CreateMap<CommentDto, Comment>().ReverseMap();

            CreateMap<Issue, IssueDto>()
                .ReverseMap();

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.CommenterName, opt => opt.MapFrom(src => src.Commenter.Name))
                .ReverseMap();

        }

    }
}
