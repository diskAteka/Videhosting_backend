namespace SharedLib.Contracts.Admin
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool CanUpload { get; set; }

        public string PasswordSalt { get; set; } = null!;

        public DateTime RegisteredAt { get; set; }

        public bool IsActive { get; set; }
    }

    public class PostUserDto
    {
        public string Name { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool CanUpload { get; set; }

        public string PasswordSalt { get; set; } = null!;

        public DateTime RegisteredAt { get; set; }

        public bool IsActive { get; set; }
    }
}
