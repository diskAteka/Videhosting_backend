namespace SharedLib.Contracts.RezultModels
{
    public struct UpdateRezult
    {
        public bool Success { get; }
        public string Message { get; set; } = "Успешное обновление записи";
        public List<string?>? AffectedTables { get; }

        public UpdateRezult(List<string?>? affectedTables, bool success = true)
        {
            AffectedTables = affectedTables;
            Success = success;
        }
    }
}
