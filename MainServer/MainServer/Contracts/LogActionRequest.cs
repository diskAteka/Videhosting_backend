namespace MainServer.Contracts
{
    /// <summary>
    /// Тело запроса POST /wpf/log.
    /// Отдельный DTO вместо SharedLib.Models.ServerLog: у сущности есть
    /// обязательная навигация Employee, и [ApiController] отвергал запрос
    /// с 400 «The Employee field is required» ещё до входа в метод.
    /// EmployeeId, Id и Date клиент не задаёт — их проставляет сервер.
    /// </summary>
    public class LogActionRequest
    {
        public string Text { get; set; } = null!;

        public string AffectedTable { get; set; } = null!;

        public string ActionType { get; set; } = null!;
    }
}
