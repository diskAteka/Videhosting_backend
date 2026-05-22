namespace SharedLib.DTOmodels.RezultModels
{
    public class AddRezult(bool success, List<string> affectedTables)
    {
        public bool Success { get; } = success;
        public List<string> AffectedTables { get; } = affectedTables;
    }
}
