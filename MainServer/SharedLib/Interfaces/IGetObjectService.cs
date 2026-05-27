namespace SharedLib.Interfaces
{
    public interface IGetObjectService
    {
        public Task<List<T>> GetObjectsAsync<T>(int from, int to, bool ascending = true) where T : class , IModel;

        public Task<int> GetCountOfRecordsAsync<T>() where T : class, IModel;

        public Task<IModel> GetObjectAsync<T>(int id) where T : class, IModel;
    }
}
