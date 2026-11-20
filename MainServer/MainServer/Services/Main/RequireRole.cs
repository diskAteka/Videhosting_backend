using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MainServer.Enum;
using SharedLib.Interfaces;
using SharedLib.Models;
using System.Security.Claims;

namespace MainServer.Services.Main
{
    public class RequireRole : IAsyncAuthorizationFilter
    {
        private readonly IGetObjectService _getObjectService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RequireRole> _logger;
        private readonly UserRole[] _requiredRoles;

        public RequireRole(
            IGetObjectService getObjectService,
            ITokenService tokenService,
            ILogger<RequireRole> logger,
            UserRole[] requiredRoles)
        {
            _getObjectService = getObjectService;
            _tokenService = tokenService;
            _logger = logger;
            _requiredRoles = requiredRoles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                // Проверяем, что это токен сотрудника
                if (!_tokenService.IsEmployeeToken(context.HttpContext.User))
                {
                    _logger.LogWarning("Попытка доступа с не-сотрудническим токеном");
                    context.Result = new ForbidResult();
                    return;
                }

                var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // Получаем роль из токена
                var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(roleClaim))
                {
                    context.Result = new ForbidResult();
                    return;
                }

                // Сравниваем роль из токена с одной из требуемых
                if (!_requiredRoles.Any(role => role.ToString() == roleClaim))
                {
                    _logger.LogWarning($"Недостаточно прав. Требуется: " +
                        $"{string.Join(", ", _requiredRoles.Select(r => r.ToString()))}, текущая: {roleClaim}");
                    context.Result = new ForbidResult();
                    return;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке роли пользователя");
                context.Result = new ForbidResult();
            }
        }
    }

    public class RequireRoleAttribute : TypeFilterAttribute
    {
        public RequireRoleAttribute(UserRole requiredRole) : base(typeof(RequireRole))
        {
            Arguments = new object[] { new[] { requiredRole } };
        }

        public RequireRoleAttribute(params UserRole[] requiredRoles) : base(typeof(RequireRole))
        {
            Arguments = new object[] { requiredRoles };
        }
    }
}