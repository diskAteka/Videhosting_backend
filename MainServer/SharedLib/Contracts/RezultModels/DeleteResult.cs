namespace SharedLib.Contracts.RezultModels
{
    public class DeleteResult
    {
        public bool Success { get; }
        public string Message { get; set; } = "Успешное удаление записи";
        public List<string?>? AffectedTables { get; }

        public DeleteResult(List<string?>? affectedTables, bool success = true)
        {
            AffectedTables = affectedTables;
            Success = success;
        }
    }
}
