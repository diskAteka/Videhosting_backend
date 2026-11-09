using AutoMapper;
using SharedLib.Contracts.Admin;
using SharedLib.Models;

namespace MainServer.Services.ForService
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, GetUserResponse>();
            CreateMap<Comment, GetCommentResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.VideoTitle, opt => opt.MapFrom(src => src.Video.Name));
            CreateMap<Video, GetVideoResponse>();
            CreateMap<Like, GetLikeResponse>();
            CreateMap<Dislike, GetDislikeResponse>();
            CreateMap<View, GetViewResponse>();
            CreateMap<ServerLog, GetServerLogResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ActionType))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.UserName));

            CreateMap<PostPutCommentRequest, Comment>();
            CreateMap<PostVideoRequest, Video>();
            CreateMap<PostLikeRequest, Like>();
            CreateMap<PostDislikeRequest, Dislike>();
            CreateMap<PostViewRequest, View>();
            CreateMap<PostLikeRequest, Like>();
            CreateMap<PostPutEmployeeRequest, Employee>();
        }
    }
}
