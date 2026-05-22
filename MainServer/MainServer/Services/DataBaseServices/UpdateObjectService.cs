using MainServer.Data;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOmodels.RezultModels;
using SharedLib.GlobalInterfaces;

namespace MainServer.Services.DataBaseServices
{
    public class UpdateObjectService : IUpdateObjectService
    {
        private AppDbContext _context;
        public UpdateObjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateRezult> UpdateAsync<T>(T table) where T : class, IUpdateble //Наследование от IUpdateble гарантирует, что передаваемый объект будет либо Video, либо User, либо Comment
        {
            try
            {
                var dbSet = _context.Set<T>();
                dbSet.Update(table);
                int affectedRows = await _context.SaveChangesAsync();
                return new UpdateRezult(affectedRows > 0, AffectedTableNames());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении объекта: {ex.Message}");
                return new UpdateRezult(false, null);
            }
        }

        private List<string> AffectedTableNames()
        {
            return _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged)
                .Select(e => e.Metadata.GetTableName())
                .Distinct()
                .ToList();
        }
    }//Класс - сервис отвечающий за обновление сущностей в БД.
}
