using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Infrastructure.Repository;
using KronoGeo_Api.Interface.Repository;
using KronoGeo_Api.Interface.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.UnitOfWork
{
    public class UnitOfWork(KronoGeoDbContext context) : IUnitOfWork
    {
        #region private properties
        private readonly KronoGeoDbContext _context = context;
        private readonly Dictionary<Type, object> _repositories = new ();
        private bool _disposed = false;
        #endregion

        #region public method IRepository implementation
        /// <summary>
        /// Gets the repository for the specified entity type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRepository<T> Repository<T>() where T : class
        {
            var typeName = typeof(T);

            if (_repositories.ContainsKey(typeName))
            {
                return (Repository<T>)_repositories[typeof(T)];
            }

            var repository = new Repository<T>(_context);
            _repositories.Add(typeof(T), repository);

            return repository;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        #endregion

        #region public method IDisposable implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
