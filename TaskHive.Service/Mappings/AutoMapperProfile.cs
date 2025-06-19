using AutoMapper;
using TaskHive.Repository.Entities;
using TaskHive.Service.DTOs.Requests.Application;
using TaskHive.Service.DTOs.Requests.Category;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Requests.Membership;
using TaskHive.Service.DTOs.Requests.Payment;
using TaskHive.Service.DTOs.Responses;
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

            CreateMap<Membership, MembershipResponseDto>().ReverseMap();
            CreateMap<AddMembershipRequestDto, Membership>();
            CreateMap<UpdateMembershipRequestDto, Membership>();

            CreateMap<AddPaymentWithMembershipRequestDto, Payment>();
            CreateMap<AddPaymentWithSlotRequestDto, Payment>();
            CreateMap<Payment, PaymentResponseDto>();


        }
    }
}