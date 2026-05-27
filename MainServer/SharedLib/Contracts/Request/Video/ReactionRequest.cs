namespace SharedLib.Contracts.Request.Video
{
    public class ReactionRequest
    {
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public bool IsLike { get; set; }
    }
}
