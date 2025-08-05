using AutoMapper;
using TaskHive.Repository.Entities;
using TaskHive.Service.DTOs;
using TaskHive.Service.DTOs.Requests.Application;
using TaskHive.Service.DTOs.Requests.BlogPost;
using TaskHive.Service.DTOs.Requests.Category;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Requests.Membership;
using TaskHive.Service.DTOs.Requests.Payment;
using TaskHive.Service.DTOs.Requests.Review;
using TaskHive.Service.DTOs.Requests.SlotPurchase;
using TaskHive.Service.DTOs.Requests.User;
using TaskHive.Service.DTOs.Requests.UserMembership;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.DTOs.Responses.BlogPost;
using TaskHive.Service.DTOs.Responses.User;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskHive.Service.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Application mappings
            CreateMap<AddApplicationRequestDto, Application>()
            .ForMember(dest => dest.CVFile, opt => opt.Ignore());

            CreateMap<UpdateApplicationRequestDto, Application>()
                .ForMember(dest => dest.CVFile, opt => opt.Ignore());
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
            CreateMap<AddMembershipRequestDto, Membership>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)); // đảm bảo lấy giá trị Status

            CreateMap<UpdateMembershipRequestDto, Membership>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<AddPaymentWithMembershipRequestDto, Payment>();
            CreateMap<AddPaymentWithSlotRequestDto, Payment>();
            CreateMap<Payment, PaymentResponseDto>();

            CreateMap<AddSlotPurchaseRequestDto, SlotPurchase>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<UpdateSlotPurchaseRequestDto, SlotPurchase>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SlotPurchase, SlotPurchaseResponseDto>();

            CreateMap<Conversation, ConversationDto>()
            .ForMember(d => d.MemberIds, opt => opt.MapFrom(s => s.Members.Select(m => m.UserId)));
            CreateMap<CreateConversationDto, Conversation>();

            CreateMap<Message, MessageDto>();
            CreateMap<SendMessageDto, Message>()
                .ForMember(m => m.FileURL, opt => opt.MapFrom(src => src.FileURL));



            // User mappings
            CreateMap<FreelancerProfileDto, User>();
            CreateMap<FreelancerProfileDto, Freelancer>();
            CreateMap<ClientProfileDto, User>();
            CreateMap<ClientProfileDto, Client>();
            CreateMap<User, UserResponseDto>();
            CreateMap<AddUserMembershipRequestDto, UserMembership>();
            CreateMap<UpdateUserMembershipRequestDto, UserMembership>();
            CreateMap<UserMembership, UserMembershipResponseDto>();

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

            //BlogPost mappings

            CreateMap<BlogPostRequestDto, BlogPost>()
                .ForMember(dest => dest.BlogpostId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore());

            CreateMap<BlogPost, BlogPostResponseDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));

            CreateMap<UpdateBlogPostDto, BlogPost>()
                .ForMember(dest => dest.BlogpostId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore());

            //Review mappings
            CreateMap<Review, ReviewResponseDto>()
                .ForMember(dest => dest.Reviewer, opt => opt.MapFrom(src => src.Reviewer))
                .ForMember(dest => dest.Reviewee, opt => opt.MapFrom(src => src.Reviewee));


                CreateMap<ReviewRequestDto, Review>()
                    .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
                    .ForMember(dest => dest.ReviewerId, opt => opt.Ignore())
                    .ForMember(dest => dest.Reviewer, opt => opt.Ignore())
                    .ForMember(dest => dest.Reviewee, opt => opt.Ignore())
                    .ForMember(dest => dest.JobPost, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<User, AllUsersResponseDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}