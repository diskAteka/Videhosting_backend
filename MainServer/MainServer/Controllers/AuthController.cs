using MainServer.Interfaces;
using MainServer.Services.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Contracts.Request.Auth;
using SharedLib.Contracts.Response.Auth;
using System.Security.Claims;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                RegisterResponse result = await _authService.RegisterAsync(request);//Сервис сам выбросит исключение если произойдет ошибка
                return Ok(result);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при регистрации");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType) , new {message = ex.Message});
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                LoginResponse rezult = await _authService.LoginAsync(request);
                return Ok(rezult);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при регистрации");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new {message = ex.Message});
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(nameIdentifier))
                    return Unauthorized(new { message = "Не удалось определить идентификатор пользователя." });
                int id = int.Parse(nameIdentifier);
                GetMeResultDto rezult = await _authService.GetMeAsync(id);
                return Ok(rezult);
            }
            catch(ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при получении данных пользователя");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new {message = ex.Message});
            }

        }
    }
}
