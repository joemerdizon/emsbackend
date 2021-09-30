using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {

        public EMSDBContext _dbContext { get; set; }

        public RepositoryBase(EMSDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.SetCommandTimeout(180);
        }

        public async virtual Task<List<T>> GetAll()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async virtual Task<T> Get(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async virtual Task<T> Add(T entity)
        {

            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        public async virtual Task<T> Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async virtual Task<T> Delete(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
        public virtual void AddRange(List<T> entities)
        {

            _dbContext.Set<T>().AddRange(entities);
            _dbContext.SaveChanges();

        }

    }
}
