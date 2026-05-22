namespace SharedLib.DTOmodels.RequestModel
{
    public class ReactionRequest
    {
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public bool IsLike { get; set; }
    }
}
