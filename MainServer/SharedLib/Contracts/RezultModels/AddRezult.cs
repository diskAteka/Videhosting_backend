namespace SharedLib.Contracts.RezultModels
{
    public class AddRezult
    {
        public bool Success { get; }
        public string Message { get; set; } = "Успешное добавление записи";
        public List<string?>? AffectedTables { get; }

        public AddRezult(List<string?>? affectedTables, bool success = true)
        {
            AffectedTables = affectedTables;
            Success = success;
        }
    }
}
