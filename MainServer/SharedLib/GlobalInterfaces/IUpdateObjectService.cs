using SharedLib.DTOmodels.RezultModels;

namespace SharedLib.GlobalInterfaces
{
    public interface IUpdateObjectService
    {
        public UpdateRezult Update<T>(T table) where T : class, IUpdateble;
    }
}
