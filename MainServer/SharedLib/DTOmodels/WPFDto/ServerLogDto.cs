namespace SharedLib.DTOmodels.Response
{
    public class ServerLogDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Type { get; set; } = null!;

        public DateTime Date { get; set; }
    }

}
