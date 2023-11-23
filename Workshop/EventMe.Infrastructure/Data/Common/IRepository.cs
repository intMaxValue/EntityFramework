

using EventMe.Infrastructure.Data.Contracts;

namespace EventMe.Infrastructure.Data.Common
{
    /// <summary>
    /// Методи за достъп на данни
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Добавяне на елемент в базата данни
        /// </summary>
        /// <typeparam name="T">Тип на елемента</typeparam>
        /// <param name="entity">Елемент</param>
        /// <returns></returns>
        Task AddAsync<T>(T entity) where T : class;

        /// <summary>
        /// Изтриване на елемент в базата данни
        /// </summary>
        /// <typeparam name="T">Тип на елемента</typeparam>
        /// <param name="entity">Елемент</param>
        /// <returns></returns>
        void Delete<T>(T entity) where T : class, IDeletable;

        /// <summary>
        /// Извличане на всички елементи от таблицата
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        IQueryable<T> All<T>() where T : class;

        /// <summary>
        /// Извличане на всички елементи от таблицата, включително изтритите
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        IQueryable<T> AllWithDeleted<T>() where T : class, IDeletable;


        /// <summary>
        /// Извличане на всички елементи от таблицата за четене
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        IQueryable<T> AllReadonly<T>() where T : class;

        /// <summary>
        /// Извличане на всички елементи от таблицата за четене, включително изтритите
        /// </summary>
        /// <typeparam name="T">Тип на елементите</typeparam>
        /// <returns></returns>
        IQueryable<T> AllWithDeletedReadonly<T>() where T : class;

        /// <summary>
        /// Запис на промените в базата данни
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
    }
}
