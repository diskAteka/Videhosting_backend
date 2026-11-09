namespace SharedLib.Contracts.Admin
{
    public class GetCommentResponse
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime Date { get; set; }

        // Имя автора комментария (Comment.User.Name) — для карточки CommentItemControl
        public string? UserName { get; set; }

        // Название видео (Comment.Video.Name) — для карточки CommentItemControl
        public string? VideoTitle { get; set; }
    }

    public class PostPutCommentRequest
    {
        public int VideoId { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}
