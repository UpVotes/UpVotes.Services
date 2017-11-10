using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using UpVotes.DataModel.IRepository;
using UpVotes.DataModel.Utility;

namespace UpVotes.DataModel.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Private member variables...
        internal UpVotesEntities Context;

        internal DbSet<T> DbSet;
        #endregion

        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }

        #region Public Constructor...
        /// <summary>
        /// Public Constructor,initializes privately declared local variables.
        /// </summary>
        /// <param name="context"></param>
        public Repository(UpVotesEntities context)
        {
            this.Context = context;
            this.DbSet = context.Set<T>();
        }
        #endregion

        public bool Add(T entity)
        {
            try
            {
                DbSet.Add(entity);
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method Add :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public bool AddRange(IEnumerable<T> entities)
        {
            try
            {
                Context.Set<T>().AddRange(entities);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method AddRange :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public bool Update(T entity)
        {
            try
            {
                DbSet.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method Update :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                //Context.Set<T>().Remove(entity);
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
                DbSet.Remove(entity);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method Delete :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public bool Delete(Func<T, Boolean> where)
        {
            try
            {
                IQueryable<T> objects = DbSet.Where<T>(where).AsQueryable();
                foreach (T obj in objects)
                    DbSet.Remove(obj);

                return true;
            }
            catch(Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method Delete :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public bool DeleteRange(IEnumerable<T> entities)
        {
            try
            {
                Context.Set<T>().RemoveRange(entities);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method DeleteRange :- PublishMessage. Error=" + ex.Message + ""), ex);
                return false;
            }
        }

        public T GetByID(int id)
        {
            try
            {
                return Context.Set<T>().Find(id);
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method GetByID :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return Context.Set<T>().Where(predicate);
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method Find :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }

        public IEnumerable<T> GetAll()
        {
            try
            {
                return Context.Set<T>().ToList();
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method GetAll :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }

        public IEnumerable<T> GetMany(Func<T, bool> where)
        {
            try
            {
                return DbSet.Where(where).ToList();
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method GetMany :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }

        public IQueryable<T> GetManyQueryable(Func<T, bool> where)
        {
            try
            {
                return DbSet.Where(where).AsQueryable();
            }
            catch (Exception ex)
            {
                Log.WriteError(string.Format("Error occured in method GetManyQueryable :- PublishMessage. Error=" + ex.Message + ""), ex);
                throw ex;
            }
        }

        public T Get(Func<T, bool> where)
        {
            return DbSet.Where(where).FirstOrDefault<T>();
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return Context.Database.SqlQuery<T>(query, parameters);
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query)
        {
            return Context.Database.SqlQuery<T>(query);
        }

        public System.Data.Entity.Infrastructure.DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters)
        {
            return Context.Database.SqlQuery<T>(sql, parameters);
        }
    }
}
