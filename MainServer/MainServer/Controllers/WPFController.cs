using Amazon.S3.Model;
using MainServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOmodels.RequestModel;
using SharedLib.Enums;
using SharedLib.GlobalClasses;
using SharedLib.GlobalInterfaces;
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


        private readonly Dictionary<Type, Func<Task<int>>> _countDelegates;

        public WPFController(IAddObjectService addObjectService,
                             IDeleteObjectService deleteObjectService,
                             IUpdateObjectService updateObjectService,
                             IGetObjectService getObjectService,
                             IVideoService videoService,
                             IConfiguration configuration,
                             ILogger<WPFController> logger)
        {
            _addObjectService = addObjectService;
            _deleteObjectService = deleteObjectService;
            _updateObjectService = updateObjectService;
            _getObjectService = getObjectService;
            _videoService = videoService;
            _configuration = configuration;
            _logger = logger;


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
        public async Task<IActionResult> UpdateVideo([FromRoute] int id, [FromBody] Video video)
        {
            try
            {
                var response = await _updateObjectService.UpdateAsync(video);
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
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] User user)
        {
            try
            {
                var response = await _updateObjectService.UpdateAsync(user);
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
                _logger.LogError(ex, "Ошибка при обновлении пользователя с id {Id}", id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }


        [Route("update/comment/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] Comment comment)
        {
            try
            {
                if (Request.Headers["X-Admin-Key"] != _configuration["X-Admin-Key"])
                    return Forbid();
                var response = await _updateObjectService.UpdateAsync(comment);
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

        #endregion

        #region add endpoints

        [Route("add/employee")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            try
            {
                var response = await _addObjectService.AddAsync(employee);
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
                _logger.LogError(ex, "Ошибка при добавлении сотрудника с id {Id}", employee.Id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/video")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddVideo([FromBody] Video video)
        {
            try
            {
                var response = await _addObjectService.AddAsync(video);
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
                _logger.LogError(ex, "Ошибка при добавлении видео с id {Id}", video.Id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/comment")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            try
            {
                var response = await _addObjectService.AddAsync(comment);
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
                _logger.LogError(ex, "Ошибка при добавлении комментария с id {Id}", comment.Id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/like")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddLike([FromBody] Like like)
        {
            try
            {
                var response = await _addObjectService.AddAsync(like);
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
                _logger.LogError(ex, "Ошибка при добавлении лайка с id {Id}", like.Id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/dislike")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddDislike([FromBody] DisLike dislike)
        {
            try
            {
                var response = await _addObjectService.AddAsync(dislike);
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
                _logger.LogError(ex, "Ошибка при добавлении дизлайка с id {Id}", dislike.Id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/view")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddView([FromBody] View view)
        {
            try
            {
                var response = await _addObjectService.AddAsync(view);
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
                _logger.LogError(ex, "Ошибка при добавлении просмотра с id {Id}", view.Id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        [Route("add/user")]
        [HttpPost]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            try
            {
                var response = await _addObjectService.AddAsync(user);
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
                _logger.LogError(ex, "Ошибка при добавлении пользователя с id {Id}", user.Id);
                return StatusCode(500, new { Message = "Неожиданная ошибка сервера" });
            }
        }

        #endregion

        #region getAll endpoints

        [HttpGet]
        [Route("get/employees")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetEmloyee([FromBody] ListObjectRequestDto request)
        {
            try
            {
                var employee = await _getObjectService.GetObjectsAsync<Employee>(request.From, request.To, request.Ascending);
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
        [Route("get/videos")]
        public async Task<IActionResult> GetVideos([FromBody] ListObjectRequestDto request)
        {
            try
            {
                var videos = await _getObjectService.GetObjectsAsync<Video>(request.From, request.To, request.Ascending);
                if (videos != null)
                    return Ok(videos);
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
        [Route("get/users")]
        public async Task<IActionResult> GetUsers([FromBody] ListObjectRequestDto request)
        {
            try
            {
                var users = await _getObjectService.GetObjectsAsync<User>(request.From, request.To, request.Ascending);
                if (users != null)
                    return Ok(users);
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
        [Route("get/comments")]
        public async Task<IActionResult> GetComments([FromBody] ListObjectRequestDto request)
        {
            try
            {
                var comments = await _getObjectService.GetObjectsAsync<Comment>(request.From, request.To, request.Ascending);
                if (comments != null)
                    return Ok(comments);
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
        [Route("get/likes")]
        public async Task<IActionResult> GetLikes([FromBody] ListObjectRequestDto request)
        {
            try
            {
                var likes = await _getObjectService.GetObjectsAsync<Like>(request.From, request.To, request.Ascending);
                if (likes != null)
                    return Ok(likes);
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
        [Route("get/dislikes")]
        public async Task<IActionResult> GetDislikes([FromBody] ListObjectRequestDto request)
        {
            try
            {
                var dislikes = await _getObjectService.GetObjectsAsync<DisLike>(request.From, request.To, request.Ascending);
                if (dislikes != null)
                    return Ok(dislikes);
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
        [Route("get/views")]
        public async Task<IActionResult> GetViews([FromBody] ListObjectRequestDto request)
        {
            try
            {
                var views = await _getObjectService.GetObjectsAsync<View>(request.From, request.To, request.Ascending);
                if (views != null)
                    return Ok(views);
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



    }
}
