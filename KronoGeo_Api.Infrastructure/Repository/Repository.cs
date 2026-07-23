using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Repository
{
    public class Repository<T>(KronoGeoDbContext context) : IRepository<T> where T : class
    {
        #region public properties
        private readonly KronoGeoDbContext _context = context;
        #endregion

        #region public methods interface implementation IRepository
        public virtual T Add(T entity)
        {
            return _context.Set<T>().Add(entity).Entity;
        }

        public virtual void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsEnumerable();
        }

        public virtual T? GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual void Update(T entity)
        {
            _context.Update(entity);
        }

        public virtual IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _context.Set<T>().Where(predicate).AsEnumerable();
        }
        #endregion
    }
}
