using KronoGeo_Api.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<T> Repository<T>() where T : class;
        public int SaveChanges();
    }
}
