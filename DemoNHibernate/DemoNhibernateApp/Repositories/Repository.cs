﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Metadata;
using NHibernate.Type;

namespace DemoNhibernateApp.Repositories
{
    public class Repository : IRepositiory
    {
        #region properties

        private ISession session;

        #endregion

        public Repository(ISession _session)
        {
            session = _session;
        }

        #region methods

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
            try
            {
                session.SaveOrUpdate(obj);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void Insert<T>(IList<T> obj) where T : class
        {
            foreach (var x1 in obj)
            {
                Insert(x1);
            }
        }

        public bool Delete<T>(T obj) where T : class
        {
            try
            {
                session.Delete(obj);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void Delete<T>(IList<T> obj) where T : class
        {
            foreach (var tmp in obj)
            {
                Delete(tmp);
            }
        }

        public bool Update<T>(T obj) where T : class
        {
            try
            {
                session.Update(obj);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void Update<T>(T obj, Expression<Func<T, object>> field) where T : class
        {
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
            IClassMetadata classMetadata =  session.SessionFactory.GetClassMetadata(typeof(T));
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
                    Console.WriteLine("Commit Failed");
                }
            }
            session.Clear();
        }

        public void Close()
        {
            session.Close();
            
        }

        #endregion

    }
}
