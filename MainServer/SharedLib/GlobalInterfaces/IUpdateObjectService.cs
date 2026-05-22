using SharedLib.DTOmodels.RezultModels;

namespace SharedLib.GlobalInterfaces
{
    public interface IUpdateObjectService
    {
        public Task<UpdateRezult> UpdateAsync<T>(T table) where T : class, IUpdateble;
    }
}
