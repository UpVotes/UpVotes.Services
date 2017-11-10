using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UpVotes.DataModel.IRepository
{
    public interface IRepository<T> where T : class
    {
        bool Add(T entity);

        bool AddRange(IEnumerable<T> entities);

        bool Update(T entity);

        bool Delete(T entity);

        bool Delete(Func<T, bool> where);

        bool DeleteRange(IEnumerable<T> entities);

        T GetByID(int id);

        IEnumerable<T> GetAll();

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetMany(Func<T, bool> where);

        IQueryable<T> GetManyQueryable(Func<T, bool> where);

        T Get(Func<T, Boolean> where);

        IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);

        IEnumerable<T> ExecWithStoreProcedure(string query);
    }
}
