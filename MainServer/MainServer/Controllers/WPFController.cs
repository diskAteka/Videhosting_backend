using Amazon.S3;
using AutoMapper;
using MainServer.Contracts;
using MainServer.Enum;
using MainServer.Interfaces;
using MainServer.Services.Main;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Contracts.Admin;
using SharedLib.Interfaces;
using SharedLib.Models;
using System.Security.Claims;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("wpf")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class WPFController : ControllerBase
    {
        private readonly IAddObjectService _addObjectService;
        private readonly IDeleteObjectService _deleteObjectService;
        private readonly IUpdateObjectService _updateObjectService;
        private readonly IGetObjectService _getObjectService;
        private readonly IVideoService _videoService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WPFController> _logger;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        private readonly string _garageEndpoint;
        private readonly string _bucketName;


        private readonly Dictionary<Type, Func<Task<int>>> _countDelegates;

        public WPFController(IAddObjectService addObjectService,
                             IDeleteObjectService deleteObjectService,
                             IUpdateObjectService updateObjectService,
                             IGetObjectService getObjectService,
                             IVideoService videoService,
                             IConfiguration configuration,
                             ILogger<WPFController> logger,
                             IStorageService storageService,
                             IMapper mapper,
                             IAuthService authService)
        {
            _addObjectService = addObjectService;
            _deleteObjectService = deleteObjectService;
            _updateObjectService = updateObjectService;
            _getObjectService = getObjectService;
            _videoService = videoService;
            _configuration = configuration;
            _logger = logger;
            _storageService = storageService;
            _mapper = mapper;
            _authService = authService;


            _garageEndpoint = configuration["Garage:Endpoint"] ?? "";
            _bucketName = configuration["Garage:BucketName"] ?? "";


            _countDelegates = new Dictionary<Type, Func<Task<int>>>//Словарь для хранения делегатов позволяет обойти ограничение на строгое указание типа в вызове метода GetCountOfRecordsAsync<T>()
                                                                   //и позволяет динамически вызывать нужный метод в зависимости от типа объекта, который нужно посчитать
            {
                { typeof(User), async () => await _getObjectService.GetCountOfRecordsAsync<User>() },
                { typeof(Video), async () => await _getObjectService.GetCountOfRecordsAsync<Video>() },
                { typeof(Comment), async () => await _getObjectService.GetCountOfRecordsAsync<Comment>() },
                { typeof(Like), async () => await _getObjectService.GetCountOfRecordsAsync<Like>() },
                { typeof(Dislike), async () => await _getObjectService.GetCountOfRecordsAsync<Dislike>() },
                { typeof(View), async () => await _getObjectService.GetCountOfRecordsAsync<View>() },
                { typeof(Employee), async () => await _getObjectService.GetCountOfRecordsAsync<Employee>() },
                { typeof(ServerLog), async () => await _getObjectService.GetCountOfRecordsAsync<ServerLog>() }
            };
        }


        #region delete endpoints
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        [Route("delete/comment/{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var response = await _deleteObjectService.DeleteAsync<Comment>(id);
            return Ok(response);
        }

        [Route("delete/like/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteLike([FromRoute] int id)
        {
            var response = await _deleteObjectService.DeleteAsync<Like>(id);
            return Ok(response);
        }

        [Route("delete/view/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteView([FromRoute] int id)
        {
            var response = await _deleteObjectService.DeleteAsync<View>(id);
            return Ok(response);
        }

        [Route("delete/dislike/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteDislike([FromRoute] int id)
        {
            var response = await _deleteObjectService.DeleteAsync<Dislike>(id);
            return Ok(response);
        }

        [Route("delete/user/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var response = await _deleteObjectService.DeleteAsync<User>(id);
            return Ok(response);
        }

        [Route("delete/video/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteVideo([FromRoute] int id)
        {
            var response = await _videoService.DeleteVideoAsync(id);
            return Ok(response);
        }

        [Route("delete/employee/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            var employee = await _getObjectService.GetObjectAsync<Employee>(id) as Employee;
            if (employee == null)
                return NotFound(new { Message = $"Сотрудник с id {id} не найден" });
            var response = await _deleteObjectService.DeleteAsync<Employee>(id);
            return Ok(response);
        }
        #endregion

        #region update endpoints
        [Route("update/video/{id}")]
        [HttpPut]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateVideo([FromRoute] int id, [FromBody] PostVideoRequest video)
        {
            var request = _mapper.Map<Video>(video);
            request.Id = id;
            var response = await _updateObjectService.UpdateAsync(request);
            return Ok(response);
        }

        [Route("update/user/{id}")]
        [HttpPut]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] PutUserRequest request)
        {
            var response = await _authService.ForcedUserUpdateAsync(id, request);
            return Ok(response);                 
        }


        [Route("update/employee/{id}")]
        [HttpPut]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] PostPutEmployeeRequest employee)
        {
            var request = _mapper.Map<Employee>(employee);
            request.Id = id;
            var response = await _updateObjectService.UpdateAsync(request);
            return Ok(response);
        }

        [Route("update/comment/{id}")]
        [RequireRole(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] PostPutCommentRequest comment)
        {
            var request = _mapper.Map<Comment>(comment);
            request.Id = id;
            var response = await _updateObjectService.UpdateAsync(request);
            return Ok(response);
        }

        [Route("reset/password/{id}")]
        [RequireRole(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> ForcedResetPassword([FromRoute] int id, [FromBody] ForcedPasswordResetRequest request)
        {
            var response = await _authService.ForcedPasswordResetAsync(id, request);
            return Ok(response);
        }

        #endregion

        #region add endpoints

        [Route("add/employee")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddEmployee([FromBody] PostPutEmployeeRequest employee)
        {
            var request = _mapper.Map<Employee>(employee);
            var response = await _addObjectService.AddAsync(request);
            return Ok(response);
        }

        [Route("add/video")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddVideo([FromBody] PostVideoRequest video)
        {
            var request = _mapper.Map<Video>(video);
            var response = await _addObjectService.AddAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Загрузка видео из AdminPanel: файлы уходят на сервер, он сам
        /// кладёт видео и постер в облачное хранилище.
        /// </summary>
        [Route("add/video/upload")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddVideoUpload([FromForm] AdminVideoUploadRequest request)
        {
            var response = await _videoService.NewVideoFromAdminAsync(request);
            return Ok(response);
        }

        [Route("add/comment")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddComment([FromBody] PostPutCommentRequest comment)
        {
            var request = _mapper.Map<Comment>(comment);
            var response = await _addObjectService.AddAsync(request);
            return Ok(response);
        }

        [Route("add/like")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddLike([FromBody] PostLikeRequest like)
        {
            var request = _mapper.Map<Like>(like);
            var response = await _addObjectService.AddAsync(request);
            return Ok(response);
        }

        [Route("add/dislike")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddDislike([FromBody] PostDislikeRequest dislike)
        {
            var request = _mapper.Map<Dislike>(dislike);
            var response = await _addObjectService.AddAsync(request);
            return Ok(response);
        }

        [Route("add/view")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddView([FromBody] PostViewRequest view)
        {
            var request = _mapper.Map<View>(view);
            var response = await _addObjectService.AddAsync(request);
            return Ok(response);
        }

        [Route("add/user")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddUser([FromBody] PostUserRequest request)
        {
            var response = await _authService.ForcedUserAddAsync(request);
            return Ok(response);
        }

        #endregion

        #region getAll endpoints

        [HttpGet]
        [Route("get/employees")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetEmloyee([FromQuery] QueryParameters parameters)
        {
            var employee = await _getObjectService.GetObjectsAsync<Employee>(parameters);
            return Ok(employee);
        }

        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/videos")]
        public async Task<IActionResult> GetVideos([FromQuery] QueryParameters parameters)
        {
            var videos = await _getObjectService.GetObjectsAsync<Video>(parameters);           
            var response = _mapper.Map<List<GetVideoResponse>>(videos);
            return Ok(response);
        }

        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/users")]
        public async Task<IActionResult> GetUsers([FromQuery] QueryParameters parameters)
        {
            var users = await _getObjectService.GetObjectsAsync<User>(parameters);
            var response = _mapper.Map<List<GetUserResponse>>(users);
            return Ok(response);
        }


        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/comments")]
        public async Task<IActionResult> GetComments([FromQuery] QueryParameters parameters)
        {
            var comments = await _getObjectService.GetObjectsAsync<Comment>(parameters);
            var response = _mapper.Map<List<GetCommentResponse>>(comments);
            return Ok(response);
        }

        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/likes")]
        public async Task<IActionResult> GetLikes([FromQuery] QueryParameters parameters)
        {
            var likes = await _getObjectService.GetObjectsAsync<Like>(parameters);
            var response = _mapper.Map<List<GetLikeResponse>>(likes);
            return Ok(response);
        }


        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/dislikes")]
        public async Task<IActionResult> GetDislikes([FromQuery] QueryParameters parameters)
        {
            var dislikes = await _getObjectService.GetObjectsAsync<Dislike>(parameters);
            var response = _mapper.Map<List<GetDislikeResponse>>(dislikes);
            return Ok(response);
        }

        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/views")]
        public async Task<IActionResult> GetViews([FromQuery] QueryParameters parameters)
        {
            var views = await _getObjectService.GetObjectsAsync<View>(parameters);
            var response = _mapper.Map<List<GetViewResponse>>(views);
            return Ok(response);
        }


        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/logs")]
        public async Task<IActionResult> GetLogs([FromQuery] QueryParameters parameters)
        {
            var logs = await _getObjectService.GetObjectsAsync<ServerLog>(parameters);
            var response = _mapper.Map<List<GetServerLogResponse>>(logs);
            return Ok(response);
        }


        [HttpGet]
        [RequireRole(UserRole.Admin)]
        [Route("get/user/videos")]
        public async Task<IActionResult> GetUserVideos([FromQuery] int userId)
        {
            var userVideos = await _videoService.GetThisUserVideos(userId);
            var mappedResponse = _mapper.Map<List<GetVideoResponse>>(userVideos);
            return Ok(mappedResponse);
        }


        #endregion

        #region virifier endpoints

        [HttpGet]
        [RequireRole(UserRole.Verifier)]
        [Route("virifier/videos")]
        public async Task<IActionResult> GetUnverifiedVideos([FromQuery] QueryParameters parameters)
        {
            var videos = await _videoService.GetUnverifiedVideosAsync(parameters);
            return Ok(videos);
        }

        #endregion

        #region getCount endpoints

        [HttpGet]
        [Route("admin/get/count")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetCount(
            [FromQuery] string type,
            [FromQuery] string? searchName,
            [FromQuery] string? searchText,
            [FromQuery] string? actionType)
        {
            if (string.IsNullOrEmpty(type))
                return BadRequest(new { Message = "Параметр type обязателен" });
            if (!TableToClassMapper.TableToClassMap.ContainsKey(type))
                return BadRequest(new { Message = $"Неверный тип объекта: {type}" });
            var objectType = TableToClassMapper.TableToClassMap[type];

            var parameters = new QueryParameters
            {
                SearchName = searchName,
                SearchText = searchText,
                ActionType = actionType
            };

            int count;
            if (objectType == typeof(Video))
                count = await _getObjectService.GetCountOfRecordsAsync<Video>(parameters);
            else if (objectType == typeof(User))
                count = await _getObjectService.GetCountOfRecordsAsync<User>(parameters);
            else if (objectType == typeof(Comment))
                count = await _getObjectService.GetCountOfRecordsAsync<Comment>(parameters);
            else if (objectType == typeof(Employee))
                count = await _getObjectService.GetCountOfRecordsAsync<Employee>(parameters);
            else if (objectType == typeof(ServerLog))
                count = await _getObjectService.GetCountOfRecordsAsync<ServerLog>(parameters);
            else
                count = await _countDelegates[objectType]();

            return Ok(new { Count = count });
        }

        [HttpGet]
        [Route("virifier/get/count")]
        [RequireRole(UserRole.Verifier)]
        public async Task<IActionResult> GetCountForVirifier([FromQuery] string type)
        {
            if (string.IsNullOrEmpty(type))
                return BadRequest(new { Message = "Параметр type обязателен" });
            if (!TableToClassMapper.TableToClassMap.ContainsKey(type))
                return BadRequest(new { Message = $"Неверный тип объекта: {type}" });
            var objectType = TableToClassMapper.TableToClassMap[type];
            if (objectType == typeof(Employee) || objectType == typeof(ServerLog))
                return Forbid();
            int count = await _countDelegates[objectType]();
            return Ok(new { Count = count });
        }
        #endregion

        #region getOne endponts

        [HttpGet]
        [Route("get/comment/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            var comment = await _getObjectService.GetObjectAsync<Comment>(id);
            var response = _mapper.Map<GetCommentResponse>(comment);
            return Ok(response);
        }

        [HttpGet]
        [Route("get/user/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _getObjectService.GetObjectAsync<User>(id);
            var response = _mapper.Map<GetUserResponse>(user);  
            return Ok(response);
        }



        [HttpGet]
        [Route("get/video/{id}")]
        [RequireRole(UserRole.Admin, UserRole.Verifier)]
        public async Task<IActionResult> GetVideo([FromRoute] int id)
        {
            var video = await _getObjectService.GetObjectAsync<Video>(id);
            var response = _mapper.Map<GetVideoResponse>(video);    
            return Ok(response);
        }


        [HttpGet]
        [Route("get/log/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetServerLog([FromRoute] int id)
        {
            var serverLog = await _getObjectService.GetObjectAsync<ServerLog>(id);
            var response = _mapper.Map<GetServerLogResponse>(serverLog);
            return Ok(response);
        }


        [HttpGet]
        [Route("get/employee/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetEmployee([FromRoute] int id)
        {
            var response = await _getObjectService.GetObjectAsync<Employee>(id);
            return Ok(response);
        }

        [HttpGet]
        [Route("get/view/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetView([FromRoute] int id)
        {
            var view = await _getObjectService.GetObjectAsync<View>(id);
            var response = _mapper.Map<GetViewResponse>(view);
            return Ok(response);
        }


        [HttpGet]
        [Route("get/like/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetLike([FromRoute] int id)
        {
            var like = await _getObjectService.GetObjectAsync<Like>(id);
            var response = _mapper.Map<GetLikeResponse>(like);
            return Ok(response);
        }


        [HttpGet]
        [Route("get/dislike/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetDislike([FromRoute] int id)
        {
            var dislike = await _getObjectService.GetObjectAsync<Dislike>(id);
            var response = _mapper.Map<GetDislikeResponse>(dislike);
            return Ok(response);
        }

        #endregion

        #region stream endpoint

        [HttpGet]
        [RequireRole(UserRole.Admin, UserRole.Verifier)]
        [Route("stream/{id}")]
        public async Task<IActionResult> StreamVideo([FromRoute] int id)
        {
            var video = await _videoService.GetVideoMetadataAsync(id);
            var url = await _storageService.GeneratePresignedUrlAsync(video.Link, expiryHours: 1);
            return Ok(url);
        }

        #endregion

        #region getPoster endpont

        [HttpGet]
        [Route("video/poster/{videoId}")]
        [RequireRole(UserRole.Admin, UserRole.Verifier)]
        public async Task<IActionResult> GetPoster([FromRoute] int videoId)
        {
            var video = await _videoService.GetVideoMetadataAsync(videoId);

            // Старые записи хранят локальные пути (C:\...) или несуществующий /posters/default.jpg.
            // Такие значения ключами в хранилище быть не могут — в S3 не ходим, отдаём 404.
            if (!IsStorageKey(video.Poster, "posters"))
            {
                _logger.LogWarning("Некорректный ключ постера в БД: «{Poster}» (videoId={VideoId})", video.Poster, videoId);
                return NotFound();
            }

            try
            {
                var response = await _storageService.GetObjectAsync(_bucketName, video.Poster);
                return new FileStreamResult(response.ResponseStream, response.Headers.ContentType ?? "image/jpeg");
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound || ex.ErrorCode == "NoSuchKey")
            {
                // Объект удалён из хранилища или запись устарела — постера просто нет.
                _logger.LogWarning("Постер отсутствует в хранилище: {Poster} (videoId={VideoId})", video.Poster, videoId);
                return NotFound();
            }
        }

        /// <summary>
        /// Проверяет, что значение из БД похоже на ключ объекта в хранилище
        /// с ожидаемым префиксом (например, "posters/{guid}.jpg").
        /// </summary>
        private static bool IsStorageKey(string? value, string prefix)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Локальные пути Windows (C:\...), UNC и любые абсолютные пути ключами не являются
            if (value.Contains('\\') || value.Contains(':') || value.StartsWith('/'))
                return false;

            return value.StartsWith($"{prefix}/", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region verify video endpoint

        [HttpPut]
        [Route("video/verify/{id}")]
        [RequireRole(UserRole.Verifier)]
        public async Task<IActionResult> VerifyVideo([FromRoute] int id)
        {
            var response = await _videoService.VerifyVideoAsync(id);
            return Ok(response);
        }

        #endregion

        #region log endpoint

        [HttpPost]
        [Route("log")]
        [RequireRole(UserRole.Admin, UserRole.Verifier)]
        public async Task<IActionResult> LogAction([FromBody] LogActionRequest request)
        {
            // Авторство — только из токена. EmployeeId/Id/Date из тела игнорируем.
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var employeeId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Text) || string.IsNullOrWhiteSpace(request.AffectedTable))
                return BadRequest(new { Message = "Text и AffectedTable обязательны" });

            // CHECK-констрейнт в БД допускает только эти значения
            if (request.ActionType is not ("INSERT" or "UPDATE" or "DELETE"))
                return BadRequest(new { Message = $"Недопустимый ActionType: {request.ActionType}" });

            var log = new ServerLog
            {
                Text = request.Text,
                AffectedTable = request.AffectedTable,
                ActionType = request.ActionType,
                EmployeeId = employeeId,
                Date = DateTime.Now
            };

            var response = await _addObjectService.AddAsync(log);
            return Ok(response);
        }

        #endregion

        #region employee login endpoint

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> EmployeeLogin([FromBody] EmployeeLoginRequest request)
        {
            string token = await _authService.EmployeeLoginAsync(request);
            return Ok(new { Token = token });
        }

        #endregion
    }
}
