namespace SharedLib.Contracts.RezultModels
{
    public class DeleteResult(bool success, List<string?>? affectedTables)
    {
        public bool Success { get; } = success;
        public List<string?>? AffectedTables { get; } = affectedTables;
    }//Результат удаления записей представлен в виде структуру для сохранения семантики кода
}
