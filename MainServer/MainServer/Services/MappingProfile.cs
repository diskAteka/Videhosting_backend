using AutoMapper;
using SharedLib.Contracts.Admin;
using SharedLib.Contracts.Request.Auth;
using SharedLib.Models;

namespace MainServer.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, GetUserResponse>();
            CreateMap<Comment, GetCommentResponse>();
            CreateMap<Video, GetVideoResponse>();
            CreateMap<Like, GetLikeResponse>();
            CreateMap<DisLike, GetDislikeResponse>();
            CreateMap<View, GetViewResponse>();
            CreateMap<ServerLog, GetServerLogResponse>();

            CreateMap<PostPutCommentRequest, Comment>();
            CreateMap<PostVideoRequest, Video>();
            CreateMap<PostLikeRequest, Like>();
            CreateMap<PostDislikeRequest, DisLike>();
            CreateMap<PostViewRequest, View>();
            CreateMap<PostLikeRequest, Like>();
            CreateMap<PostPutEmployeeRequest, Employee>();
        }
    }
}
