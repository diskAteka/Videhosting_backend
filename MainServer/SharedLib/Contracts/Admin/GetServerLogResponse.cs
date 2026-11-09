namespace SharedLib.Contracts.Admin
{
    public class GetServerLogResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Type { get; set; } = null!;

        public DateTime Date { get; set; }

        public string Text { get; set; } = null!;

        public string AffectedTable { get; set; } = null!;

        // БАГ 45: имя сотрудника, внёсшего запись (Employee.UserName).
        public string? EmployeeName { get; set; }

        // Отображаемые строки для WPF: AffectedTable хранится как "Video"/"User"/"Comment"/"Employee".
        public string AffectedTableDisplay => AffectedTable switch
        {
            "Video" => "Видео",
            "User" => "Пользователи",
            "Comment" => "Комментарии",
            "Employee" => "Сотрудники",
            _ => AffectedTable
        };

        // "#5 (Имя Смирнов)" или "#5", если имя не загрузилось.
        public string EmployeeDisplay =>
            string.IsNullOrEmpty(EmployeeName) ? $"#{UserId}" : $"#{UserId} ({EmployeeName})";
    }
}
