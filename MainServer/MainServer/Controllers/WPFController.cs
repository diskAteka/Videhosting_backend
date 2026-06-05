using AutoMapper;
using MainServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Common;
using SharedLib.Contracts.Admin;
using SharedLib.Enums;
using SharedLib.Interfaces;
using SharedLib.Models;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("wpf")]
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
                { typeof(DisLike), async () => await _getObjectService.GetCountOfRecordsAsync<DisLike>() },
                { typeof(View), async () => await _getObjectService.GetCountOfRecordsAsync<View>() },
                { typeof(Employee), async () => await _getObjectService.GetCountOfRecordsAsync<Employee>() },
                {typeof(ServerLog), async () => await _getObjectService.GetCountOfRecordsAsync<ServerLog>() }
            };
        }


        #region delete endpoints
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        [Route("delete/comment/{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            try
            {
                var response = await _deleteObjectService.DeleteAsync<Comment>(id);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при удалении комментария с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("delete/like/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteLike([FromRoute] int id)
        {
            try
            {
                var response = await _deleteObjectService.DeleteAsync<Like>(id);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);

            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при удалении лайка с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("delete/view/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteView([FromRoute] int id)
        {
            try
            {
                var response = await _deleteObjectService.DeleteAsync<View>(id);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при удалении просмотра с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("delete/dislike/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteDislike([FromRoute] int id)
        {
            try
            {
                var response = await _deleteObjectService.DeleteAsync<DisLike>(id);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при удалении дизлайка с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("delete/user/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            try
            {
                var response = await _deleteObjectService.DeleteAsync<User>(id);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при удалении пользователя с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("delete/video/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteVideo([FromRoute] int id)
        {
            try
            {
                var response = await _videoService.DeleteVideoAsync(id);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при удалении видео с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("delete/employee/{id}")]
        [HttpDelete]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            try
            {
                var employee = await _getObjectService.GetObjectAsync<Employee>(id) as Employee;
                if (employee == null)
                    return NotFound(new { Message = $"Сотрудник с id {id} не найден" });
                var response = await _deleteObjectService.DeleteAsync<Employee>(id);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при удалении сотрудника с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }
        #endregion

        #region update endpoints
        [Route("update/video/{id}")]
        [HttpPut]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateVideo([FromRoute] int id, [FromBody] PostVideoRequest video)
        {
            try
            {
                var request = _mapper.Map<Video>(video);
                var response = await _updateObjectService.UpdateAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при обновлении видео с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("update/user/{id}")]
        [HttpPut]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] PutUserRequest request)
        {
            try
            {
                var response = await _authService.ForcedUserUpdateAsync(id, request);
                if (response.Success)
                    return Ok(response);
                else
                {
                    response.Message = response.Message ?? "Неожиданная ошибка при обновлении пользователя";
                    return StatusCode(500, response);
                }                   
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при обновлении пользователя с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }


        [Route("update/comment/{id}")]
        [RequireRole(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] PostPutCommentRequest comment)
        {
            try
            {
                if (Request.Headers["X-Admin-Key"] != _configuration["X-Admin-Key"])
                    return Forbid();
                var request = _mapper.Map<Comment>(comment);
                var response = await _updateObjectService.UpdateAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при обновлении комментария с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("reset/password/{id}")]
        [RequireRole(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> ForcedResetPassword([FromRoute] int id, [FromBody] ForcedPasswordResetRequest request)
        {
            try
            {
                var response = await _authService.ForcedPasswordResetAsync(id, request);
                if (response.Success)
                    return Ok(response);
                else
                {
                    response.Message = response.Message ?? "Неожиданная ошибка при принудительной смене пароля";
                    return StatusCode(500, response);
                }                   
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при принудительной смене пароля для пользователя с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        #endregion

        #region add endpoints

        [Route("add/employee")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddEmployee([FromBody] PostPutEmployeeRequest employee)
        {
            try
            {
                var request = _mapper.Map<Employee>(employee);
                var response = await _addObjectService.AddAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении сотрудника");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/video")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddVideo([FromBody] PostVideoRequest video)
        {
            try
            {
                var request = _mapper.Map<Video>(video);
                var response = await _addObjectService.AddAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении видео");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/comment")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddComment([FromBody] PostPutCommentRequest comment)
        {
            try
            {
                var request = _mapper.Map<Comment>(comment);
                var response = await _addObjectService.AddAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении комментария");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/like")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddLike([FromBody] PostLikeRequest like)
        {
            try
            {
                var request = _mapper.Map<Like>(like);
                var response = await _addObjectService.AddAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении лайка");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/dislike")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddDislike([FromBody] PostDislikeRequest dislike)
        {
            try
            {
                var request = _mapper.Map<DisLike>(dislike);
                var response = await _addObjectService.AddAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении дизлайка");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/view")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddView([FromBody] PostViewRequest view)
        {
            try
            {
                var request = _mapper.Map<View>(view);
                var response = await _addObjectService.AddAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении просмотра");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/user")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddUser([FromBody] PostUserRequest request)
        {
            try
            {
                var response = await _authService.ForcedUserAddAsync(request);
                if (response.Success)
                    return Ok(response);
                else
                {
                    response.Message = response.Message ?? "Неожиданная ошибка при добавлении пользователя";
                    return StatusCode(500, response);
                }
                    
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении комментария");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        #endregion

        #region getAll endpoints

        [HttpGet]
        [Route("get/employees")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetEmloyee([FromQuery] QueryParameters parameters)
        {
            try
            {
                var employee = await _getObjectService.GetObjectsAsync<Employee>(parameters);
                if (employee != null)
                    return Ok(employee);
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного сотрудника" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении сотрудников ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/videos")]
        public async Task<IActionResult> GetVideos([FromQuery] QueryParameters parameters)
        {
            try
            {
                var videos = await _getObjectService.GetObjectsAsync<Video>(parameters);
                if (videos != null)
                { 
                
                    var response = _mapper.Map<List<GetVideoResponse>>(videos);
                    return Ok(response);
                }
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного видео" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении видео ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/users")]
        public async Task<IActionResult> GetUsers([FromQuery] QueryParameters parameters)
        {
            try
            {
                var users = await _getObjectService.GetObjectsAsync<User>(parameters);
                if (users != null)
                {
                    var response = _mapper.Map<List<GetUserResponse>>(users);
                    return Ok(response);
                }
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного пользователя" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении пользователей ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }


        [HttpGet]
        [Authorize]
        [Route("get/comments")]
        public async Task<IActionResult> GetComments([FromQuery] QueryParameters parameters)
        {
            try
            {
                var comments = await _getObjectService.GetObjectsAsync<Comment>(parameters);
                if (comments != null)
                {
                    var response = _mapper.Map<List<GetCommentResponse>>(comments);
                    return Ok(response);
                }
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного комментария" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении комментариев ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/likes")]
        public async Task<IActionResult> GetLikes([FromQuery] QueryParameters parameters)
        {
            try
            {
                var likes = await _getObjectService.GetObjectsAsync<Like>(parameters);
                if (likes != null)
                {
                    var response = _mapper.Map<List<GetLikeResponse>>(likes);
                    return Ok(response);
                }
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного лайка" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении лайков ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/dislikes")]
        public async Task<IActionResult> GetDislikes([FromQuery] QueryParameters parameters)
        {
            try
            {
                var dislikes = await _getObjectService.GetObjectsAsync<DisLike>(parameters);
                if (dislikes != null)
                {
                    var response = _mapper.Map<List<GetDislikeResponse>>(dislikes);
                    return Ok(response);
                }
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного дизлайка" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении дизлайков ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/views")]
        public async Task<IActionResult> GetViews([FromQuery] QueryParameters parameters)
        {
            try
            {
                var views = await _getObjectService.GetObjectsAsync<View>(parameters);
                if (views != null)
                {
                    var response = _mapper.Map<List<GetViewResponse>>(views);
                    return Ok(response);
                }
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного просмотра" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении просмотров ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get/user/videos")]
        public async Task<IActionResult> GetUserVideos([FromQuery] int userId)
        {
            try
            {
                var userVideos = await _videoService.GetThisUserVideos(userId);
                if (userVideos != null)
                {
                    var mappedResponse = _mapper.Map<List<GetVideoResponse>>(userVideos);
                    return Ok(mappedResponse);
                }
                else
                    return StatusCode(500, new { Message = $"Не найдено не одного просмотра" });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении просмотров ");
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }


        #endregion

        #region getCount endpoints

        [HttpGet]
        [Route("admin/get/count")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetCount([FromQuery] string type)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                    return BadRequest(new { Message = "Параметр type обязателен" });
                if (!TableToClassMapper.TableToClassMap.ContainsKey(type))
                    return BadRequest(new { Message = $"Неверный тип объекта: {type}" });
                var objectType = TableToClassMapper.TableToClassMap[type];
                int count = await _countDelegates[objectType]();
                return Ok(new { Count = count });
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении количества объектов типа {Type}", type);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [HttpGet]
        [Route("virifier/get/count")]
        [RequireRole(UserRole.Virifier)]
        public async Task<IActionResult> GetCountForVirifier([FromQuery] string type)
        {
            try
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
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    return StatusCode(ApiException.GetStatusCode(apiEx.ErrorType), new { Message = apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при получении количества объектов типа {Type}", type);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }
        #endregion

        #region getOne endponts

        [HttpGet]
        [Route("get/comment/{id}")]
        [Authorize]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            try
            {
                var comment = await _getObjectService.GetObjectAsync<Comment>(id);
                var response = _mapper.Map<GetCommentResponse>(comment);
                return Ok(response);
            }
            catch(Exception ex)
            {
                if(ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }

        [HttpGet]
        [Route("get/user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            try
            {
                var user = await _getObjectService.GetObjectAsync<User>(id);
                var response = _mapper.Map<GetUserResponse>(user);  
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }



        [HttpGet]
        [Route("get/video/{id}")]
        [Authorize]
        public async Task<IActionResult> GetVideo([FromRoute] int id)
        {
            try
            {
                var video = await _getObjectService.GetObjectAsync<Video>(id);
                var response = _mapper.Map<GetVideoResponse>(video);    
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }


        [HttpGet]
        [Route("get/log/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetServerLog([FromRoute] int id)
        {
            try
            {
                var serverLog = await _getObjectService.GetObjectAsync<ServerLog>(id);
                var response = _mapper.Map<GetServerLogResponse>(serverLog);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }


        [HttpGet]
        [Route("get/employee/{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetEmployee([FromRoute] int id)
        {
            try
            {
                var response = await _getObjectService.GetObjectAsync<Employee>(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }

        [HttpGet]
        [Route("get/view/{id}")]
        [Authorize]
        public async Task<IActionResult> GetView([FromRoute] int id)
        {
            try
            {
                var view = await _getObjectService.GetObjectAsync<View>(id);
                var response = _mapper.Map<GetViewResponse>(view);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }


        [HttpGet]
        [Route("get/like/{id}")]
        [Authorize]
        public async Task<IActionResult> GetLike([FromRoute] int id)
        {
            try
            {
                var like = await _getObjectService.GetObjectAsync<Like>(id);
                var response = _mapper.Map<GetLikeResponse>(like);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }


        [HttpGet]
        [Route("get/dislike/{id}")]
        [Authorize]
        public async Task<IActionResult> GetDislike([FromRoute] int id)
        {
            try
            {
                var dislike = await _getObjectService.GetObjectAsync<DisLike>(id);
                var response = _mapper.Map<GetDislikeResponse>(dislike);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }

        #endregion

        #region stream endpoint

        [HttpGet]
        [Authorize]
        [Route("stream/{id}")]
        public async Task<IActionResult> StreamVideo([FromRoute] int id)
        {
            try
            {
                var video = await _videoService.GetVideoMetadataAsync(id);
                if (video == null)
                    return NotFound("Видео не найдено");

                var response = await _storageService.GetObjectAsync(_bucketName, video.Link);
                return new FileStreamResult(response.ResponseStream, response.Headers.ContentType ?? "video/mp4")
                { EnableRangeProcessing = true };
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { Message = apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }

        #endregion

        #region getPoster endpont

        [HttpGet]
        [Route("video/poster/{id}")]
        [RequireRole(UserRole.Virifier)]
        public async Task<IActionResult> GetPoster([FromRoute] int videoId)
        {
            try
            {
                var video = await _videoService.GetVideoMetadataAsync(videoId);
                if (video == null)
                    return NotFound("Видео не найдено");
                var response = await _storageService.GetObjectAsync(_bucketName, video.Poster);
                return new FileStreamResult(response.ResponseStream, response.Headers.ContentType ?? "image/jpeg");
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { apiEx.Message });
                }
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }

        #endregion

        #region verify video endpoint

        [HttpPut]
        [Route("video/verify/{id}")]
        [RequireRole(UserRole.Virifier)]
        public async Task<IActionResult> VerifyVideo([FromRoute] int id)
        {
            try
            {
                var response = await _videoService.VerifyVideoAsync(id);
                if (response.Success)
                    return Ok(response);
                else
                {
                    response.Message = response.Message ?? "Неожиданная ошибка при верификации видео";
                    return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при верификации видео с id {Id}", id);
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }

        #endregion

        #region

        [HttpPost]
        [Route("log")]
        [Authorize]
        public async Task<IActionResult> LogAction([FromBody] ServerLog log)
        {
            try
            {
                var response = await _addObjectService.AddAsync(log);
                if (response.Success)
                    return Ok(response);
                else
                {
                    response.Message = response.Message ?? "Неожиданная ошибка при добавлении лога";
                    return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var apiEx = (ApiException)ex;
                    int code = ApiException.GetStatusCode(apiEx.ErrorType);
                    return StatusCode(code, new { apiEx.Message });
                }
                _logger.LogError(ex, "Ошибка при добавлении лога");
                return StatusCode(500, new { Message = "Непредвиденная ошибка сервера" });
            }
        }

        #endregion
    }
}
