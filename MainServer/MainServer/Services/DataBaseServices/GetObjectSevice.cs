using MainServer.Data;
using Microsoft.EntityFrameworkCore;
using SharedLib.Enums;
using SharedLib.GlobalClasses;
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
                // Сортировка по Id
                list = ascending ? list.OrderBy(e => EF.Property<object>(e, "Id")) : list.OrderByDescending(e => EF.Property<object>(e, "Id"));
                return await list.Skip(from).Take(to).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Ошибка при получении объектов типа {typeof(T)}: {ex.Message}", ex);
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
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                return entity ?? throw new ApiException(ErrorType.NotFound, $"Объект типа {typeof(T)} с id {id} не найден");   
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Ошибка при получении объекта типа {typeof(T)} с id {id}: {ex.Message}", ex);
            }

        }
    }
}
