using MainServer.Data;
using MainServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Contracts.Admin;
using SharedLib.Contracts.Request.Auth;
using SharedLib.Contracts.Response.Auth;
using SharedLib.Contracts.RezultModels;
using MainServer.Enum;
using SharedLib.Interfaces;
using SharedLib.Models;
using System.Data.Common;
using MainServer.Services.Main;

namespace MainServer.Services.ForController
{
    public class AuthService : IAuthService //Этот сервис будет выполнять запросы в БД.
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext context, ILogger<AuthService> logger, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest register)
        {
            try
            {
                User? existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == register.Email);

                if (existingUser != null)
                    throw new ApiException(ErrorType.Conflict, "Пользователь с таким Email уже существует");

                (string hash, string salt) passwordhash = _passwordHasher.HashPassword(register.Password);

                User user = new User
                {
                    Name = register.UserName.Trim(),
                    Email = register.Email.ToLowerInvariant(),
                    PasswordHash = passwordhash.hash,
                    PasswordSalt = passwordhash.salt,
                    RegisteredAt = DateTime.UtcNow,
                    IsActive = true,
                    CanUpload = false
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return RegisterResponse.Ok(); 
            }
            catch (DbUpdateException ex)
            {
                throw new ApiException(ErrorType.ServerError, $"Ошибка при сохранении в БД {ex.Message}");
            }
            catch(Exception ex) when (ex is not ApiException)
            {
                _logger.LogError($"Неожиданная ошибка при регистрации {register.Email}, Exeption {ex.Message}");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }

        }//Метод добавления нового пользователя в БД


        public async Task<LoginResponse> LoginAsync(LoginRequest login)
        {
            try
            {
                var user = await _context.Users
                      .FirstOrDefaultAsync(u => u.Email == login.Email || u.Name == login.UserName);

                if (user == null)
                    throw new ApiException(ErrorType.NotFound, "Пользователь не найден");
                var testHash = _passwordHasher.GetHash(login.Password + user.PasswordSalt);
                bool isValid = _passwordHasher.VerifyPassword(
                    login.Password,
                    user.PasswordHash,
                    user.PasswordSalt);

                if (!isValid)
                    throw new ApiException(ErrorType.Unauthorized, "Неверный пароль");

                string token = _tokenService.GenerateToken(user.Id, user.Name, user.Email, user.CanUpload);
                LoginResponse loginRez = new()
                {
                    Token = token,
                    User = new()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        CanUpload = user.CanUpload,
                        RegisteredAt = user.RegisteredAt,
                        IsActive = user.IsActive
                    }
                };
                return loginRez;
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Ошибка БД при входе {Email}", login.Email);
                throw new ApiException(ErrorType.ServerError,"Ошибка при входе в систему");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError(ex, "Неожиданная ошибка при входе {Email}", login.Email);
                throw new ApiException(ErrorType.ServerError,"Внутренняя ошибка сервера");
            }
        }//Проверяет есть ли пользователь с таким логином и паролем в БД


        public async Task<GetMeResultDto> GetMeAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ApiException(ErrorType.ValidationError,$"ID должен быть больше 0. Получено: {id}");

                User? user = await _context.Users.FindAsync(id);

                if (user == null)
                    throw new ApiException(ErrorType.NotFound,$"Пользователя с ID {id} не существует");

                var result = new GetMeResultDto
                {
                    Name = user.Name,
                    Email = user.Email,
                    CanUpload = user.CanUpload,
                    IsActive = user.IsActive,
                };

                return GetMeResultDto.Ok(result);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"Ошибка БД при получении данных пользователя ID={id}", id);
                throw new ApiException(ErrorType.ServerError,"Ошибка при получении данных пользователя");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError(ex, $"Неожиданная ошибка GetMeAsync ID {id}", id);
                throw new ApiException(ErrorType.ServerError,"Внутренняя ошибка сервера");
            }
        }//Метод получение информации о пользователе


        public async Task<UpdateRezult> ForcedPasswordResetAsync(int id, ForcedPasswordResetRequest request)
        {
            try
            {
                User? existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (existingUser == null)
                    throw new ApiException(ErrorType.NotFound, $"Пользователь с ID {id} не найден");
                (string hash, string salt) encryptedPassword = _passwordHasher.HashPassword(request.NewPassword);
                existingUser.PasswordHash = encryptedPassword.hash;
                existingUser.PasswordSalt = encryptedPassword.salt;

                _context.Users.Update(existingUser);
                var affectedTables = AffectedTableNames();
                int affectedRows = await _context.SaveChangesAsync();

                return new UpdateRezult(affectedTables, affectedRows > 0);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Ошибка БД при сбросе пароля");
                throw new ApiException(ErrorType.ServerError, "Ошибка при сбросе пароля");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError(ex, "Неожиданная ошибка при сбросе пароля");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }//Метод для принудительного сброса пароля

        public async Task<string> EmployeeLoginAsync(EmployeeLoginRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Username)
                    && !string.IsNullOrEmpty(request.Password))
                {
                    var existingEmployee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.UserName == request.Username);
                    if (existingEmployee == null) throw new ApiException(ErrorType.NotFound, "Сотрудник с таким логином не найден");
                    if (existingEmployee.Password != request.Password) throw new ApiException(ErrorType.Unauthorized, "Неверный пароль");
                    string token = _tokenService.GenerateEmployeeToken(existingEmployee.Id, existingEmployee.UserName, existingEmployee.Role);
                    return token;
                }
                else throw new ApiException(ErrorType.ValidationError, "Неверный формат входящих данных");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Ошибка БД при сбросе пароля");
                throw new ApiException(ErrorType.ServerError, "Ошибка при сбросе пароля");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError(ex, "Неожиданная ошибка при сбросе пароля");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }

        public async Task<AddRezult> ForcedUserAddAsync(PostUserRequest request)
        {
            try
            {
                User? existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingUser != null)
                    throw new ApiException(ErrorType.Conflict, "Пользователь с таким Email уже существует");

                (string hash, string salt) passwordhash = _passwordHasher.HashPassword(request.Password);

                User user = new User
                {
                    Name = request.UserName.Trim(),
                    Email = request.Email.ToLowerInvariant(),
                    PasswordHash = passwordhash.hash,
                    PasswordSalt = passwordhash.salt,
                    RegisteredAt = request.RegisteredAt,
                    IsActive = request.IsActive,
                    CanUpload = request.CanUpload
                };

                await _context.Users.AddAsync(user);
                var affectedTables = AffectedTableNames();
                int affectedRows = await _context.SaveChangesAsync();

                return new AddRezult(affectedTables, affectedRows > 0);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Ошибка БД при добавлении пользователя");
                throw new ApiException(ErrorType.ServerError, "Ошибка при входе в систему");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError(ex, "Неожиданная ошибка при добавлении пользователя");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }//Метод для принудительного добавления пользователя

        public Task<UpdateRezult> ForcedUserUpdateAsync(int id, PutUserRequest request)
        {
            try
            {
                User? existingUser = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
                if (existingUser == null)
                    throw new ApiException(ErrorType.NotFound, $"Пользователь с ID {id} не найден");
                existingUser.Name = request.Name.Trim();
                existingUser.Email = request.Email.ToLowerInvariant();
                existingUser.CanUpload = request.CanUpload;
                existingUser.RegisteredAt = request.RegisteredAt;
                existingUser.IsActive = request.IsActive;
                _context.Users.Update(existingUser);
                var affectedTables = AffectedTableNames();
                int affectedRows = _context.SaveChanges();
                return Task.FromResult(new UpdateRezult(affectedTables, affectedRows > 0));
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Ошибка БД при обновлении пользователя");
                throw new ApiException(ErrorType.ServerError, "Ошибка при обновлении пользователя");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError(ex, "Неожиданная ошибка при обновлении пользователя");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }//Метод для принудительного обновления информации о пользователе

        private List<string?> AffectedTableNames()
        {
            return _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged)
                .Select(e => e.Metadata.GetTableName())
                .Distinct()
                .ToList();
        }
    }
}
