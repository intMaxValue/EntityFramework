﻿using System.Collections;

namespace MiniORM
{
    public class DbSet<TEntity> : ICollection<TEntity> where TEntity : class, new()
    {
        public IList<TEntity> Entities { get; set; }

        internal ChangeTracker<TEntity> ChangeTracker { get; set; }

        public DbSet(IEnumerable<TEntity> entities)
        {
            Entities = entities.ToList();
            ChangeTracker = new ChangeTracker<TEntity>(entities);
        }
        public int Count => Entities.Count;

        public bool IsReadOnly => Entities.IsReadOnly;

        public void Add(TEntity item)
        {
            if (item is null)
            {
                throw new ArgumentNullException("Item cannot be null");
            }
            Entities.Add(item);

            ChangeTracker.Add(item);
        }

        public void Clear()
        {
            while (Entities.Any())
            {
                var entity = Entities.First();
                
                Remove(entity);
            }
        }

        public bool Contains(TEntity item)
        {
            return Entities.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            Entities.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        public bool Remove(TEntity item)
        {
            if (item is null)
            {
                throw new ArgumentNullException("Item cannot be null");
            }

            var removedSuccessfully = Entities.Remove(item);

            if (removedSuccessfully)
            {
                ChangeTracker.Remove(item);
            }

            return removedSuccessfully;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
