namespace SharedLib.Contracts.Admin
{
    public class QueryParameters
    {
        public int Page { get; set; } = 1;   
        public int PageSize { get; set; } = 20; 
        public string? SortBy { get; set; } = "Id"; 
        public bool SortAscending { get; set; } = true;

        // БАГ 44: поиск по имени (Video.Name / User.Name / Employee.UserName) и по тексту (Comment.Text).
        // Пустые/null значения фильтры отключают.
        public string? SearchName { get; set; }
        public string? SearchText { get; set; }

        // БАГ 45: фильтр логов по типу. Допустимые значения ограничены CHECK-констрейнтом: "INSERT"/"UPDATE"/"DELETE".
        public string? ActionType { get; set; }
    }

    public class UserQueryParameters : QueryParameters
    {
        public int UserId { get; set; }
    }
}
