using SharedLib.GlobalInterfaces;

namespace SharedLib.Models
{
    public partial class Employee : IModel, IDeleteble, IAddble
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}


