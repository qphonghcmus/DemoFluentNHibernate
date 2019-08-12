using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Metadata;
using NHibernate.Type;

namespace FluentNHibernateApp.Repositories
{
    public class Repository : IRepository,IDisposable
    {
        #region properties

        private ISession session;

        #endregion

        #region private methods

        // Constructor
        public Repository(ISession _session)
        {
            session = _session;
        }

        private void Refresh()
        {
            if (session != null && !session.IsOpen)
            {
                //session.Reconnect();
                session.Connection.Open();
            }
        }

        #endregion

        #region Implemented Methods

        public T Get<T>(object id) where T : class
        {
            return session.Get<T>(id);
        }

        public IList<T> GetAll<T>() where T : class
        {
            return session.Query<T>().ToList();
        }

        public IList<T> Where<T>(Expression<Func<T, bool>> condition) where T : class
        {
            return session.Query<T>().Where(condition).ToList();
        }

        public IList<T> Where<T>(Expression<Func<T, bool>> condition, int quantity) where T : class
        {
            return session.Query<T>().Where(condition).Take(quantity).ToList();
        }

        public bool Insert<T>(T obj) where T : class
        {
            Refresh();
            try
            {
                session.SaveOrUpdate(obj);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Insert<T>(IList<T> obj) where T : class
        {
            Refresh();
            using (ITransaction transaction = session.BeginTransaction())
            {
                foreach (var x1 in obj)
                {
                    Insert(x1);
                }
                transaction.Commit();
            }
            session.Clear();
        }

        public bool InsertBulk<T>(IList<T> objs) where T : class
        {
            return true;
        }

        public bool Delete<T>(T obj) where T : class
        {
            Refresh();
            try
            {
                session.Delete(obj);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete<T>(IList<T> obj) where T : class
        {
            Refresh();
            foreach (var tmp in obj)
            {
                Delete(tmp);
            }
        }

        public void Delete<T>(Expression<Func<T, bool>> condition)
        {
            Refresh();
            session.Query<T>().Where(condition).Delete();
        }

        public bool Update<T>(T obj) where T : class
        {
            Refresh();
            try
            {
                session.Update(obj);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update<T>(T obj, Expression<Func<T, object>> field) where T : class
        {
            Refresh();
            MemberExpression m;
            if (field.Body is MemberExpression)
            {
                m = field.Body as MemberExpression;
            }
            else
            {
                m = (field.Body as UnaryExpression).Operand as MemberExpression;
            }

            object value = null;
            var allProperties = obj.GetType().GetProperties();
            foreach (var propertyInfo in allProperties)
            {
                if (propertyInfo.Name == m.Member.Name)
                {
                    value = propertyInfo.GetValue(obj);
                    break;
                }
            }

            string whereClause = "where ";
            IClassMetadata classMetadata = session.SessionFactory.GetClassMetadata(typeof(T));
            string[] proppertiesOfTable_Name = classMetadata.PropertyNames;
            IType[] proppertiesOfTable_Type = classMetadata.PropertyTypes;

            foreach (var prop in allProperties)
            {
                if (classMetadata.IdentifierPropertyName == prop.Name)
                {
                    whereClause += $"{prop.Name} = \'{prop.GetValue(obj)}\'";
                }
            }

            string sql = $"Update {obj.GetType().Name} Set {m.Member.Name} = \'{value}\' " + whereClause;

            using (var transaction = session.BeginTransaction())
            {
                session.CreateSQLQuery(sql).AddEntity(typeof(T)).UniqueResult();
                transaction.Commit();
            }
        }

        public void Commit()
        {
            Refresh();
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Flush();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    transaction.Rollback();
                    //Console.WriteLine("Commit Failed");
                    throw e;
                }
            }
            session.Clear();
        }

        public void Clear()
        {
            Refresh();
            session.Clear();
        }

        public void Close()
        {
            session.Close();
            session?.Dispose();
        }

        #endregion

        public void Dispose()
        {
            session?.Dispose();
        }
    }
}
