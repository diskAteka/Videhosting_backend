namespace SharedLib.DTOmodels.RequestModel
{
    public class ListObjectRequestDto
    {
        public int From { get; set; } = 0;
        public int To { get; set; } = 20;
        public bool Ascending { get; set; } = true; // Сортировка по возрастанию по умолчанию
    }

    public class GetUserVideosRequest : ListObjectRequestDto
    {
        public int UserId { get; set; } 
    }
}
