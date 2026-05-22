using SharedLib.DTOmodels.RezultModels;

namespace SharedLib.GlobalInterfaces
{
    public interface IDeleteObjectService
    {
        //Этот интерфейс должен отвечать за удаление любых сущностей в БД

        public DeleteResult Delete<T>(int id) where T : class, IDeleteble;
    }
}
