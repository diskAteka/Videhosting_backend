using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SharedLib.Enums;
using SharedLib.GlobalInterfaces;
using SharedLib.Models;
using System.Security.Claims;

namespace SharedLib.GlobalClasses
{
    public class RequireRole : IAsyncAuthorizationFilter
    {
        private readonly IGetObjectService _getObjectService;
        private readonly ILogger<RequireRole> _logger;
        private readonly UserRole _requiredRole;

        public RequireRole(IGetObjectService getObjectService, ILogger<RequireRole> logger, UserRole requiredRole)
        {
            _getObjectService = getObjectService;
            _logger = logger;
            _requiredRole = requiredRole;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var userIdCliam = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdCliam))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                int userId = int.Parse(userIdCliam);
                Employee? employee = await _getObjectService.GetObjectAsync<Employee>(userId) as Employee;
                if (employee == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }                
                if (employee.Role != _requiredRole.ToString())
                {
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
            Arguments = new object[] { requiredRole };
        }
    }
}
