using SharedLib.GlobalInterfaces;

namespace SharedLib.DTOmodels.Response
{
    public class LikeDto
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public int UserId { get; set; }
    }

    public class PostLikeDto
    {
        public int VideoId { get; set; }
        public int UserId { get; set; }
    }
}
