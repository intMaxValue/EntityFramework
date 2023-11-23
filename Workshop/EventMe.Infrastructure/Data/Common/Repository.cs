
using EventMe.Infrastructure.Data.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EventMe.Infrastructure.Data.Common
{
    public class Repository : IRepository
    {
        private readonly EventMeDbContext dbContext;
        /// <summary>
        /// Конструктор за инжектиране на контекста на базата данни
        /// </summary>
        /// <param name="_dbContext">Контекста на базата данни</param>
        public Repository(EventMeDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        /// <summary>
        /// Връща DbSet за даден тип
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private DbSet<T> DbSet<T>() where T : class => dbContext.Set<T>();

        /// <summary>
        /// Добавяне на елемент в базата данни
        /// </summary>
        /// <typeparam name="T">Тип на елемента</typeparam>
        /// <param name="entity">Елемент</param>
        /// <returns></returns>
        public async Task AddAsync<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        /// <summary>
        /// Изтриване на елемент в базата данни
        /// </summary>
        /// <typeparam name="T">Тип на елемента</typeparam>
        /// <param name="entity">Елемент</param>
        /// <returns></returns>
        public void Delete<T>(T entity) where T : class, IDeletable
        {
            entity.IsActive = false;
            entity.DeletedOn = DateTime.Now;
        }

        /// <summary>
        /// Извличане на всички елементи от таблицата
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        public IQueryable<T> All<T>() where T : class
        {
            return DbSet<T>();
        }

        /// <summary>
        /// Извличане на всички елементи от таблицата, включително изтритите
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        public IQueryable<T> AllWithDeleted<T>() where T : class, IDeletable
        {
            return DbSet<T>()
                .IgnoreQueryFilters()
                .AsNoTracking();
        }

        /// <summary>
        /// Извличане на всички елементи от таблицата за четене
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        public IQueryable<T> AllReadonly<T>() where T : class
        {
            return DbSet<T>()
                .AsNoTracking();
        }

        /// <summary>
        /// Извличане на всички елементи от таблицата за четене, включително изтритите
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        public IQueryable<T> AllWithDeletedReadonly<T>() where T : class
        {
            return DbSet<T>()
                .IgnoreQueryFilters()
                .AsNoTracking();
        }

        /// <summary>
        /// Запис на промените в базата данни
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
