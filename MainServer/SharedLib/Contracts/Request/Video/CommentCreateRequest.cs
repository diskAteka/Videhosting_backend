namespace SharedLib.Contracts.Request.Video
{
    public class CommentCreateRequest
    {
        public string? Text { get; set; }
    }

    public class CommentCreateDto
    {
        public int UserId { get; set; }
        public string? Text { get; set; }
        public int VideoId { get; set; }
    }
}
