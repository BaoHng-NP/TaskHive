using AutoMapper;
using TaskHive.Repository.Entities;
using TaskHive.Service.DTOs.Requests.Application;
using TaskHive.Service.DTOs.Requests.Category;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Requests.User;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.DTOs.Responses.User;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskHive.Service.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Application mappings
            CreateMap<AddApplicationRequestDto, Application>();
            CreateMap<UpdateApplicationRequestDto, Application>();
            CreateMap<Application, ApplicationResponseDto>()
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.FullName))
                .ForMember(dest => dest.JobPostTitle, opt => opt.MapFrom(src => src.JobPost.Title));

            // Category mappings
            CreateMap<AddCategoryRequestDto, Category>();
            CreateMap<UpdateCategoryRequestDto, Category>();
            CreateMap<Category, CategoryResponseDto>();

            // JobPost mappings
            CreateMap<AddJobPostRequestDto, JobPost>();
            CreateMap<UpdateJobPostRequestDto, JobPost>();
            CreateMap<JobPost, JobPostResponseDto>()
                .ForMember(dest => dest.EmployerName, opt => opt.MapFrom(src => src.Employer.FullName))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));


            // User mappings
            CreateMap<FreelancerProfileDto, User>();
            CreateMap<ClientProfileDto, User>();
            // Direct mapping from derived entities to response DTOs
            CreateMap<Freelancer, FreelancerProfileResponseDto>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.UserSkills))
                .IncludeBase<User, FreelancerProfileResponseDto>();

            CreateMap<Client, ClientProfileResponseDto>()
                .IncludeBase<User, ClientProfileResponseDto>();

            // Base mapping for common User properties
            CreateMap<User, FreelancerProfileResponseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl));

            CreateMap<User, ClientProfileResponseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl));

            CreateMap<UserSkill, UserSkillDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CategoryDescription, opt => opt.MapFrom(src => src.Category.Description));

        }
    }
}