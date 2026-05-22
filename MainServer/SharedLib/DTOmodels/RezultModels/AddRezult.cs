namespace SharedLib.DTOmodels.RezultModels
{
    public class AddRezult(bool Success, List<string?>? AffectedTables)
    {
        public bool Success { get; } = Success;
        public List<string?>? AffectedTables { get; } = AffectedTables;
    }
}
