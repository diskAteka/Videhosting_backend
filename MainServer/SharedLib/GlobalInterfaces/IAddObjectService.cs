using SharedLib.DTOmodels.RezultModels;

namespace SharedLib.GlobalInterfaces
{
    public interface IAddObjectService
    {
        public AddRezult Add<T>(T table) where T : class, IAddble; 
        //Наследование от интерфейса IAdd гарантирует что не будут переданы объекты,
        //которые не предназначены для добавления в базу данных
    }
}
