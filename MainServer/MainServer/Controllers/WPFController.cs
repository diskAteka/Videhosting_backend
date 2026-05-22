using MainServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
                    return StatusCode(500,response);
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

        #region get endpoints

        #endregion
    }
}
