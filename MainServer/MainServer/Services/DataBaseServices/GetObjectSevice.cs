using MainServer.Data;
using MainServer.Enum;
using MainServer.Services.Main;
using Microsoft.EntityFrameworkCore;
using SharedLib.Contracts.Admin;
using SharedLib.Interfaces;
using SharedLib.Models;
using System.Linq.Dynamic.Core;

namespace MainServer.Services.DataBaseServices
{
    public class GetObjectSevice : IGetObjectService
    {
        private AppDbContext _context;

        public GetObjectSevice(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetObjectsAsync<T>(QueryParameters parameters) where T : class, IModel
        {
            // Include навигаций там, где контракту нужны связанные данные
            IQueryable<T> query;
            if (typeof(T) == typeof(ServerLog) && _context.Set<T>() is IQueryable<ServerLog> logQuery)
            {
                // БАГ 45: Employee.UserName → EmployeeName
                query = (IQueryable<T>)logQuery.Include(l => l.Employee).AsNoTracking();
            }
            else if (typeof(T) == typeof(Comment) && _context.Set<T>() is IQueryable<Comment> commentQuery)
            {
                // UserName/VideoTitle для карточек комментариев
                query = (IQueryable<T>)commentQuery
                    .Include(c => c.User)
                    .Include(c => c.Video)
                    .AsNoTracking();
            }
            else
            {
                query = _context.Set<T>().AsNoTracking();
            }

            // БАГ 44/45: поисковые фильтры до сортировки и пагинации
            query = ApplySearchFilters<T>(query, parameters);
            query = ApplySortingDynamic(query, parameters);

            int skip = (parameters.Page - 1) * parameters.PageSize;
            var result = await query
                .Skip(skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            return result;
        }

        public async Task<int> GetCountOfRecordsAsync<T>() where T : class, IModel
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> GetCountOfRecordsAsync<T>(QueryParameters parameters) where T : class, IModel
        {
            var query = ApplySearchFilters<T>(_context.Set<T>().AsNoTracking(), parameters);
            return await query.CountAsync();
        }

        public async Task<IModel> GetObjectAsync<T>(int id) where T : class, IModel
        {
            // БАГ 45: одиночный лог тоже возвращаем с именем сотрудника
            if (typeof(T) == typeof(ServerLog) && _context.Set<T>() is DbSet<ServerLog> logSet)
            {
                var serverLog = await logSet
                    .AsNoTracking()
                    .Include(l => l.Employee)
                    .FirstOrDefaultAsync(l => l.Id == id);

                return serverLog ?? throw new ApiException(ErrorType.NotFound, $"Объект типа {typeof(T)} с id {id} не найден");
            }

            var entity = await _context.Set<T>().FindAsync(id);
            return entity ?? throw new ApiException(ErrorType.NotFound, $"Объект типа {typeof(T)} с id {id} не найден");
        }

        private IQueryable<T> ApplySortingDynamic<T>(IQueryable<T> query, QueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return parameters.SortAscending
                    ? query.OrderBy("Id ASC")
                    : query.OrderBy("Id DESC");
            }

            string orderByString = $"{parameters.SortBy} {(parameters.SortAscending ? "ASC" : "DESC")}";

            try
            {
                return query.OrderBy(orderByString);
            }
            catch
            {
                return query.OrderBy("Id ASC");
            }
        }

        // БАГ 44/45: фильтры зависят от типа сущности. Пустые значения отключают фильтры.
        private static IQueryable<T> ApplySearchFilters<T>(IQueryable<T> query, QueryParameters parameters) where T : class, IModel
        {
            if (typeof(T) == typeof(Video) &&
                query is IQueryable<Video> videos && !string.IsNullOrWhiteSpace(parameters.SearchName))
            {
                var name = parameters.SearchName.Trim();
                query = (IQueryable<T>)videos.Where(v => v.Name.Contains(name));
            }

            if (typeof(T) == typeof(User) &&
                query is IQueryable<User> users && !string.IsNullOrWhiteSpace(parameters.SearchName))
            {
                var name = parameters.SearchName.Trim();
                query = (IQueryable<T>)users.Where(u => u.Name.Contains(name));
            }

            if (typeof(T) == typeof(Employee) &&
                query is IQueryable<Employee> employees && !string.IsNullOrWhiteSpace(parameters.SearchName))
            {
                var name = parameters.SearchName.Trim();
                query = (IQueryable<T>)employees.Where(e => e.UserName.Contains(name));
            }

            if (typeof(T) == typeof(Comment) &&
                query is IQueryable<Comment> comments && !string.IsNullOrWhiteSpace(parameters.SearchText))
            {
                var text = parameters.SearchText.Trim();
                query = (IQueryable<T>)comments.Where(c => c.Text.Contains(text));
            }

            if (typeof(T) == typeof(ServerLog) &&
                query is IQueryable<ServerLog> logs && !string.IsNullOrWhiteSpace(parameters.ActionType))
            {
                var actionType = parameters.ActionType.Trim().ToUpperInvariant();
                query = (IQueryable<T>)logs.Where(l => l.ActionType == actionType);
            }

            return query;
        }
    }
}
