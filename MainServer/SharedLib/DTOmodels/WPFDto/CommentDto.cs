namespace SharedLib.DTOmodels.Response
{
    public class CommentDto
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime Date { get; set; }
    }

    public class PostCommentDto
    {
        public int VideoId { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}
