using SharedLib.Contracts.Request.Auth;
using SharedLib.Contracts.Response.Auth;

namespace MainServer.Interfaces
{
    public interface IAuthService
    {
        public Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        public Task<LoginResponse> LoginAsync(LoginRequest request);
        public Task<GetMeResultDto> GetMeAsync(int id);
    }//Интерфес описывает логику класса сервиса. Все методы асинхронные потому что они все будут выполнять запросы к БД.
}
