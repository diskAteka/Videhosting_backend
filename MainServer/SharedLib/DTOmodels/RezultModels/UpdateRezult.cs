namespace SharedLib.DTOmodels.RezultModels
{
    public struct UpdateRezult(bool success, List<string>? affectedTables)
    {
        public bool Success { get; } = success;
        public List<string>? AffectedTables { get; } = affectedTables;
    }
}
