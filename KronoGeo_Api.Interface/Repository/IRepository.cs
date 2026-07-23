using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Repository
{
    public interface IRepository <T> where T : class 
    {
        public IEnumerable<T> GetAll();
        public T? GetById(int id);
        public T Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public IEnumerable<T> Where(Func<T, bool> predicate);
    }
}
