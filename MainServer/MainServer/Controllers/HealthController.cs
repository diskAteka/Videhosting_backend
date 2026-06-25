using MainServer.Data;
using MainServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace MainServer.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IStorageService _storageService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            AppDbContext dbContext,
            IStorageService storageService,
            ILogger<HealthController> logger)
        {
            _dbContext = dbContext;
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckServices()
        {
            var result = new
            {
                Status = "OK",
                Timestamp = DateTime.UtcNow,
                Database = new { Status = "Unknown", Message = "" },
                Storage = new { Status = "Unknown", Message = "" }
            };
            try
            {
                bool canConnect = await _dbContext.Database.CanConnectAsync();
                if (canConnect)
                    result = result with { Database = new { Status = "Healthy", Message = "Подключение к базе данных успешно" } };
                else
                {
                    result = result with { Database = new { Status = "Unhealthy", Message = "Не удалось подключиться к базе данных" } };
                    return StatusCode(503, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке базы данных");
                result = result with { Database = new { Status = "Error", Message = $"Ошибка: {ex.Message}" } };
                return StatusCode(503, result);
            }
            try
            {
                bool storageAvailable = await _storageService.CheckStorageAvailabilityAsync();
                if (storageAvailable)
                    result = result with { Storage = new { Status = "Healthy", Message = "Облачное хранилище доступно" } };
                else
                {
                    result = result with { Storage = new { Status = "Unhealthy", Message = "Облачное хранилище недоступно" } };
                    return StatusCode(503, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке облачного хранилища");
                result = result with { Storage = new { Status = "Error", Message = $"Ошибка: {ex.Message}" } };
                return StatusCode(503, result);
            }
            return Ok(result);
        }
    }
}