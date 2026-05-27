using MainServer.Data;
using Microsoft.EntityFrameworkCore;
using SharedLib.Common;
using SharedLib.Interfaces;
using SharedLib.Enums;
using SharedLib.Contracts.RezultModels;

namespace MainServer.Services.DataBaseServices
{
    public class DeleteObjectService : IDeleteObjectService
    {
        private readonly AppDbContext _context;
        public DeleteObjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DeleteResult> DeleteAsync<T>(int id) where T : class, IDeleteble
        {
            try
            {
                var dbSet = _context.Set<T>(); //Определяети тип таблицы на лету
                var entity = await dbSet.FindAsync(id);
                if (entity == null)
                    throw new ApiException(ErrorType.NotFound, $"Объект типа {typeof(T)} с id {id} не найден");
                dbSet.Remove(entity);
                int affectedRows = await _context.SaveChangesAsync();
                return new DeleteResult(affectedRows > 0, AffectedTableNames()); //Если удаление прошло успешно, возвращаем результат с информацией об измененных таблицах
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении объекта типа {typeof(T)}, {ex.Message}");
                return new DeleteResult(false, null); //При возникновении ошибки возвращаем неудачный результат с пустым массивом
            }
        }
        private List<string?> AffectedTableNames()
        {
            return _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged)
                .Select(e => e.Metadata.GetTableName())
                .Distinct()
                .ToList();
        }
    }//Класс - сервис отвечающий за удаление сущностей из БД.
     //Реализует интерфейс IDeleteObjectService, который определяет методы для удаления пользователей, видео и комментариев.
     //Каждый метод обрабатывает удаление соответствующей сущности и возвращает результат в виде структуры DeleteResult,
     //которая содержит информацию об успешности операции и затронутых таблицах.
}
