using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext Context = context;
        private readonly DbSet<T> DbSet = context.Set<T>();

        public virtual async Task<T?> GetByIdAsync(int id)
            => await DbSet.FindAsync(id);

        public virtual async Task<T> AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            // Only call update for detached entities
            // only update the context if the entity is not being track
            if (Context.Entry(entity).State == EntityState.Detached)
                DbSet.Update(entity);

            await context.SaveChangesAsync();

            return entity;
        }
    }
}
