using MainServer.Data;
using Microsoft.EntityFrameworkCore;
using SharedLib.GlobalInterfaces;

namespace MainServer.Services.DataBaseServices
{
    public class GetObjectSevice : IGetObjectService
    {
        private AppDbContext _context;
        public GetObjectSevice(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<T>> GetObjectsAsync<T>(int from = 0, int to = 10, bool ascending = true) where T : class, IModel
        {
            IQueryable<T>? list = null;
            try
            {
                list = _context.Set<T>().AsNoTracking();

                // Применяем сортировку по Id
                list = ascending ? list.OrderBy(e => EF.Property<object>(e, "Id")) : list.OrderByDescending(e => EF.Property<object>(e, "Id"));

                return await list.Skip(from).Take(to).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<int> GetCountOfRecordsAsync<T>() where T : class, IModel
        {
            try
            {
                return await _context.Set<T>().CountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<IModel> GetObjectAsync<T>(int id) where T : class, IModel
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
