using SharedLib.DTOmodels;

namespace SharedLib.ServerClasses
{
    public class LoginResultDto
    {
        public string? Token { get; set; }
        public UserResponseDto? User {  get; set; }
    }
}
