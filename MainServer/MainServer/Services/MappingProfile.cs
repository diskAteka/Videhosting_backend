using AutoMapper;
using SharedLib.Contracts.Admin;
using SharedLib.Models;

namespace MainServer.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Comment, CommentDto>();
            CreateMap<Video, VideoDto>();
            CreateMap<Like, LikeDto>();
            CreateMap<DisLike, DislikeDto>();
            CreateMap<View, ViewDto>();
            CreateMap<ServerLog, ServerLogDto>();

            CreateMap<PostCommentDto, Comment>();
            CreateMap<PostVideoDto, Video>();
            CreateMap<PostLikeDto, Like>();
            CreateMap<PostDislikeDto, DisLike>();
            CreateMap<PostViewDto, View>();
            CreateMap<PostLikeDto, Like>();
            CreateMap<PostPutEmployeeRequest, Employee>();
        }
    }
}
