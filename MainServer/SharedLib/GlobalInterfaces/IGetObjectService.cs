namespace SharedLib.GlobalInterfaces
{
    public interface IGetObjectService
    {
        List<T> GetObjects<T>(int from, int to, bool ascending = true) where T : class , IModel;

        int GetCountOfRecords<T>() where T : class, IModel;

        public IModel GetObject<T>(int id) where T : class, IModel;
    }
}
