using MainServer.Data;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOmodels.RezultModels;
using SharedLib.GlobalInterfaces;

namespace MainServer.Services.DataBaseServices
{
    public class AddObjectService : IAddObjectService
    {
        private AppDbContext _context;
        public AddObjectService(AppDbContext context)
        {
            _context = context;
        }

        public AddRezult Add<T>(T table) where T : class, IAddble
        {
            try
            {
                var dbSet = _context.Set<T>();
                dbSet.Add(table);
                int affectedRows = _context.SaveChanges();
                return new AddRezult(affectedRows > 0, AffectedTableNames());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении объекта: {ex.Message}");
                return new AddRezult(false, null);
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
    }//Класс - сервис отвечающий за добавление сущностей в БД.
     //Реализует интерфейс IAddObjectService, который определяет методы для добавления записей в любые таблицы, кроме таблицы ServerLog.
     //Generic метод реализует добавление сущности и возвращает результат в виде структуры AddRezult,
     //которая содержит информацию об успешности операции и затронутых таблицах.
}
