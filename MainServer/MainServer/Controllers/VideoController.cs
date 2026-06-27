using MainServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Contracts.Request.Video;
using SharedLib.Contracts.Response.Video;
using System.Security.Claims;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IStorageService _storageService;

        public VideoController(IVideoService videoService, IStorageService storageService)
        {
            _videoService = videoService;
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideo()
        {
            List<VideoListItemResponse> videos = await _videoService.GetAllVideoAsync();
            return Ok(videos);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchVideo([FromQuery] string query)
        {
            List<VideoListItemResponse> videos = await _videoService.VideoSearchAsync(query);
            return Ok(videos);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetVideo(int id)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier))
                return Unauthorized(new { message = "Не удалось определить идентификатор пользователя." });

            int userId = int.Parse(nameIdentifier);
            VideoDetailsResponse video = await _videoService.GetVideoAsync(id, userId);
            return Ok(video);
        }

        [HttpPost("{id}/reaction")]
        [Authorize]
        public async Task<IActionResult> Reaction(int id, [FromBody] ReactionDto reaction)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier))
                return Unauthorized(new { message = "Не удалось определить идентификатор пользователя." });

            int userId = int.Parse(nameIdentifier);
            ReactionRequest request = new ReactionRequest
            {
                UserId = userId,
                IsLike = reaction.IsLike,
                VideoId = id
            };

            await _videoService.ReactionAsync(request);
            return Ok();
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetVideoComment(int id)
        {
            List<CommentResponse> comments = await _videoService.GetCommentsAsync(id);
            return Ok(comments);
        }

        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> NewComment([FromBody] CommentCreateRequest comment, int id)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier))
                return Unauthorized(new { message = "Не удалось определить идентификатор пользователя." });

            int userId = int.Parse(nameIdentifier);
            CommentCreateDto request = new()
            {
                UserId = userId,
                Text = comment.Text,
                VideoId = id
            };

            await _videoService.NewCommentAsync(request);
            return Ok(request);
        }

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamVideo(int id)
        {
            var video = await _videoService.GetVideoMetadataAsync(id);
            var url = await _storageService.GeneratePresignedUrlAsync(video.Link, expiryHours: 1);
            return Redirect(url);
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadRequest request)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier))
                return Unauthorized(new { message = "Не удалось определить идентификатор пользователя." });

            int userId = int.Parse(nameIdentifier);
            int newVideoId = await _videoService.NewVideoAsync(request, userId);
            VideoDetailsResponse video = await _videoService.GetVideoAsync(newVideoId, userId);
            return Ok(video);
        }

        [HttpGet("upload")]
        [Authorize]
        public async Task<IActionResult> GetUserVideos()
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier))
                return Unauthorized(new { message = "Не удалось определить идентификатор пользователя." });

            int userId = int.Parse(nameIdentifier);
            List<UserVideoListItemDto> videos = await _videoService.GetThisUserVideos(userId);
            return Ok(videos);
        }
    }
}