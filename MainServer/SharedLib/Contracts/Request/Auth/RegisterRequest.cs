using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SharedLib.Contracts.Request.Auth
{
    public class RegisterRequest
    {
        [Required, MinLength(3), MaxLength(50)] public string UserName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6), MaxLength(16)] public string Password { get; set; } = string.Empty;
    }
}
