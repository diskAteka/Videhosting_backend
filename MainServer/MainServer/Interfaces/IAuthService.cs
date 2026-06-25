using SharedLib.Contracts.Admin;
using SharedLib.Contracts.Request.Auth;
using SharedLib.Contracts.Response.Auth;
using SharedLib.Contracts.RezultModels;

namespace MainServer.Interfaces
{
    public interface IAuthService
    {
        public Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        public Task<LoginResponse> LoginAsync(LoginRequest request);
        public Task<GetMeResultDto> GetMeAsync(int id);
        public Task<UpdateRezult>ForcedPasswordResetAsync(int id, ForcedPasswordResetRequest request);
        public Task<AddRezult> ForcedUserAddAsync(PostUserRequest request);
        public Task<UpdateRezult> ForcedUserUpdateAsync(int id, PutUserRequest request);
        public Task<string> EmployeeLoginAsync(EmployeeLoginRequest request);
    }//Интерфес описывает логику класса сервиса. Все методы асинхронные потому что они все будут выполнять запросы к БД.
}
