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
        public List<T> GetObjects<T>(int from = 0, int to = 10, bool ascending = true) where T : class, IModel
        {
            IQueryable<T>? list = null;
            try
            {
                list = _context.Set<T>().AsNoTracking();

                // Применяем сортировку по Id
                list = ascending ? list.OrderBy(e => EF.Property<object>(e, "Id")) : list.OrderByDescending(e => EF.Property<object>(e, "Id"));

                return list.Skip(from).Take(to).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public int GetCountOfRecords<T>() where T : class, IModel
        {
            try
            {
                return _context.Set<T>().Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public IModel GetObject<T>(int id) where T : class, IModel
        {
            return _context.Set<T>().Find(id);
        }
    }
}
