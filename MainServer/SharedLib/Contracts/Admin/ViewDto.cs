namespace SharedLib.Contracts.Admin
{
    public class ViewDto
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public int UserId { get; set; }
    }

    public class PostViewDto
    {
        public int VideoId { get; set; }
        public int UserId { get; set; }
    }
}
