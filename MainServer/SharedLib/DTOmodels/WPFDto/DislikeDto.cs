namespace SharedLib.DTOmodels.Response
{
    public class DislikeDto
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public int UserId { get; set; }
    }

    public class PostDislikeDto
    {
        public int VideoId { get; set; }
        public int UserId { get; set; }
    }
}
