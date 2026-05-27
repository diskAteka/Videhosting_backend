namespace SharedLib.Contracts.Admin
{
    public class VideoDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime DateUpload { get; set; }

        public string Link { get; set; } = null!;

        public string Poster { get; set; } = null!;

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public bool IsVerified { get; set; }

        public int Views { get; set; }

        public int AuthorId { get; set; }
    }

    public class PostVideoDto
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime DateUpload { get; set; }

        public string Link { get; set; } = null!;

        public string Poster { get; set; } = null!;

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public bool IsVerified { get; set; }

        public int Views { get; set; }

        public int AuthorId { get; set; }
    }
}
